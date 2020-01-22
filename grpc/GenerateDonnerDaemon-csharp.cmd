cd proto

..\bin\csharp\windows_x64\protoc.exe -I. -I../bin/csharp/windows_x64 -I../include/ -I%GOPATH%/src  --csharp_out ../csharp --grpc_out ../csharp --plugin=protoc-gen-grpc=../bin/csharp/windows_x64/grpc_csharp_plugin.exe donnerrpc.proto


cmd /k