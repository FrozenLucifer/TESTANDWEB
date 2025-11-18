import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend, Counter, Rate, Gauge } from 'k6/metrics';

const TEST_TYPE = __ENV.TEST_TYPE;

const duration = new Trend(`${TEST_TYPE}_duration_ms`);
const errors = new Counter(`${TEST_TYPE}_errors_total`);
const successRate = new Rate(`${TEST_TYPE}_success_rate`);
const requestsCounter = new Counter(`${TEST_TYPE}_requests_total`);

export const options = {
    teardownTimeout: '5s',
    stages: [
        { duration: '35s', target: 3000 },
        { duration: '20s', target: 3000 },
        { duration: '30s', target: 200 },
        { duration: '15s', target: 0 },
    ],
    thresholds: {
        [`${TEST_TYPE}_duration_ms`]: ['p(95)<500', 'p(99)<1000'],
        [`${TEST_TYPE}_success_rate`]: ['rate>0.95'],
    },
};

export default function () {
    const productId = Math.floor(Math.random() * 100) + 1;

    let url;
    if (TEST_TYPE === 'efcore') {
        url = `http://localhost:5000/products/${productId}`;
    } else {
        url = `http://localhost:5001/products/${productId}`;
    }

    const response = http.get(url);
    requestsCounter.add(1);

    const success = check(response, {
        'status 200': (r) => r.status === 200
    });

    duration.add(response.timings.duration);
    successRate.add(success);
    if (!success) errors.add(1);

    sleep(0.1);
}