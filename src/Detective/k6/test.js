import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '10s', target: 500 },   
        { duration: '10s', target: 0 },    
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], 
        http_req_failed: ['rate<0.01'],   
    },
};

export default function () {
    let res = http.get('http://localhost/api/v1/health');
    check(res, { 'main API status 200': (r) => r.status === 200 });
    
    sleep(1);
}
