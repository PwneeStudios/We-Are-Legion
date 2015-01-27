cd %~dp0\..

start Terracotta.exe --client --ip 127.0.0.1 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --debug --double
start Terracotta.exe --server                --port 13000 --p 2 --t 1234 --n 2 --map Beset.m3n   --debug --double
