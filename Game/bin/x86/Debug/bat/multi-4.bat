cd %~dp0\..

start Terracotta.exe --server                --port 13000 --p1 --n 4   --debug --quad
start Terracotta.exe --client --ip 127.0.0.1 --port 13000 --p2 --n 4   --debug --quad
start Terracotta.exe --client --ip 127.0.0.1 --port 13000 --p3 --n 4   --debug --quad
start Terracotta.exe --client --ip 127.0.0.1 --port 13000 --p4 --n 4   --debug --quad