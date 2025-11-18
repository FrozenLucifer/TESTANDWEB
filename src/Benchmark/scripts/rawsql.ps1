k6 run --address localhost:6565 --out experimental-prometheus-rw -e TEST_TYPE=rawsql -e OPERATION=select k6/new2.js
k6 run --address localhost:6565 --out experimental-prometheus-rw -e TEST_TYPE=rawsql -e OPERATION=insert k6/new2.js
k6 run --address localhost:6565 --out experimental-prometheus-rw -e TEST_TYPE=rawsql -e OPERATION=batch_insert k6/new2.js
k6 run --address localhost:6565 --out experimental-prometheus-rw -e TEST_TYPE=rawsql -e OPERATION=delete k6/new2.js
