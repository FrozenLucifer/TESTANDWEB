k6 run --address localhost:6566 --out experimental-prometheus-rw -e TEST_TYPE=efcore -e OPERATION=select k6/new2.js
k6 run --address localhost:6566 --out experimental-prometheus-rw -e TEST_TYPE=efcore -e OPERATION=insert k6/new2.js
k6 run --address localhost:6566 --out experimental-prometheus-rw -e TEST_TYPE=efcore -e OPERATION=batch_insert k6/new2.js
k6 run --address localhost:6566 --out experimental-prometheus-rw -e TEST_TYPE=efcore -e OPERATION=delete k6/new2.js
k6 run --address localhost:6566 --out experimental-prometheus-rw -e TEST_TYPE=efcore -e OPERATION=batch_delete k6/new2.js