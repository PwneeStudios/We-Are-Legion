cd %~dp0
start python proxy.py

cd %~dp0\..
start Terracotta.exe --server                --port 13000 --p1 --n 2   --debug --double
start Terracotta.exe --client --ip 127.0.0.1 --port 13001 --p2 --n 2   --debug --double
