import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend, Counter, Rate, Gauge } from 'k6/metrics';
import { randomIntBetween, randomItem } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

const TEST_TYPE = __ENV.TEST_TYPE;
const OPERATION = __ENV.OPERATION || 'select'; // select, insert, batch_insert, delete, batch_delete

const duration = new Trend(`${TEST_TYPE}_${OPERATION}_duration_ms`);
const errors = new Counter(`${TEST_TYPE}_${OPERATION}_errors_total`);
const successRate = new Rate(`${TEST_TYPE}_${OPERATION}_success_rate`);
const requestsCounter = new Counter(`${TEST_TYPE}_${OPERATION}_requests_total`);

const batchSizeGauge = new Gauge(`${TEST_TYPE}_batch_size`);

export const options = {
    teardownTimeout: '10s',
    stages: [
        { duration: '30s', target: 100000 },
        // { duration: '20s', target: 1000 },
        // { duration: '20s', target: 0 },
        // { duration: '10s', target: 0 },
    ]
};

const BASE_URL = TEST_TYPE === 'efcore' ? 'http://localhost:5000' : 'http://localhost:5001';

export default function () {
    let success;

    switch (OPERATION) {
        case 'select':
            success = testSelect();
            break;
        case 'insert':
            success = testInsert();
            break;
        case 'batch_insert':
            success = testBatchInsert();
            break;
        case 'delete':
            success = testDelete();
            break;
        case 'batch_delete':
            success = testBatchDelete();
            break;
        default:
            success = testSelect();
    }

    requestsCounter.add(1);
    successRate.add(success);
    if (!success) errors.add(1);

    sleep(0.1);
}

function testSelect() {
    const productId = randomIntBetween(1, 1000);
    const url = `${BASE_URL}/products/${productId}`;

    const response = http.get(url);
    duration.add(response.timings.duration);

    return check(response, {
        'status 200': (r) => r.status === 200,
        'has product data': (r) => r.json('id') === productId
    });
}

function testInsert() {
    const newProduct = {
        name: `TestProduct_${Date.now()}_${Math.random()}`,
        price: randomIntBetween(10, 1000),
        categoryId: randomIntBetween(1, 10)
    };

    const url = `${BASE_URL}/products`;
    const params = {
        headers: { 'Content-Type': 'application/json' },
    };

    const response = http.post(url, JSON.stringify(newProduct), params);
    duration.add(response.timings.duration);

    return check(response, {
        'status 201': (r) => r.status === 201,
        'insert successful': (r) => r.json('id') !== undefined
    });
}

function testBatchInsert() {
    const batchSize = randomIntBetween(5, 20);
    batchSizeGauge.add(batchSize);

    const products = Array.from({ length: batchSize }, (_, i) => ({
        name: `BatchProduct_${Date.now()}_${i}`,
        price: randomIntBetween(10, 500),
        categoryId: randomIntBetween(1, 10)
    }));

    const url = `${BASE_URL}/products/batch`;
    const params = {
        headers: { 'Content-Type': 'application/json' },
    };

    const response = http.post(url, JSON.stringify(products), params);
    duration.add(response.timings.duration);

    return check(response, {
        'status 201': (r) => r.status === 201,
        'batch insert successful': (r) => r.json('insertedCount') === batchSize
    });
}

function testDelete() {
    const tempProduct = {
        name: `TempDelete_${Date.now()}`,
        price: 100,
        categoryId: 1
    };

    const createUrl = `${BASE_URL}/products`;
    const createResponse = http.post(createUrl, JSON.stringify(tempProduct), {
        headers: { 'Content-Type': 'application/json' },
    });

    if (createResponse.status !== 201) return false;

    const productId = createResponse.json('id');
    const deleteUrl = `${BASE_URL}/products/${productId}`;

    const deleteResponse = http.del(deleteUrl);
    duration.add(deleteResponse.timings.duration);

    return check(deleteResponse, {
        'status 200 or 204': (r) => r.status === 200 || r.status === 204
    });
}

function testBatchDelete() {
    const batchSize = randomIntBetween(3, 10);
    const productIds = [];

    for (let i = 0; i < batchSize; i++) {
        const tempProduct = {
            name: `TempBatchDelete_${Date.now()}_${i}`,
            price: 100,
            categoryId: 1
        };

        const createResponse = http.post(`${BASE_URL}/products`, JSON.stringify(tempProduct), {
            headers: { 'Content-Type': 'application/json' },
        });

        if (createResponse.status === 201) {
            productIds.push(createResponse.json('id'));
        }
    }

    if (productIds.length === 0) return false;

    const deleteUrl = `${BASE_URL}/products/batch`;
    const deleteResponse = http.del(deleteUrl, JSON.stringify({ ids: productIds }), {
        headers: { 'Content-Type': 'application/json' },
    });

    duration.add(deleteResponse.timings.duration);

    return check(deleteResponse, {
        'status 200': (r) => r.status === 200,
        'batch delete successful': (r) => r.json('deletedCount') === productIds.length
    });
}

export function setup() {
    const cleanupUrl = `${BASE_URL}/test/cleanup`;
    http.del(cleanupUrl);
}