cd proto

..\bin\csharp\windows_x64\protoc.exe -I. -I../bin/csharp/windows_x64 -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --csharp_out ../csharp/bbh --grpc_out ../csharp/bbh --plugin=protoc-gen-grpc=../bin/csharp/windows_x64/grpc_csharp_plugin.exe gameserver.proto
..\bin\csharp\windows_x64\protoc.exe -I. -I../bin/csharp/windows_x64 -I../include/ -I%GOPATH%/src -I%GOPATH%/src/github.com/grpc-ecosystem/grpc-gateway/third_party/googleapis --csharp_out ../csharp/bbh --grpc_out ../csharp/bbh --plugin=protoc-gen-grpc=../bin/csharp/windows_x64/grpc_csharp_plugin.exe client.proto


cmd /k