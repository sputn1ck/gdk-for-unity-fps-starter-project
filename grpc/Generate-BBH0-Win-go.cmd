cd proto

protoc -I. -I../include/ -I%GOPATH%/src --go_out=plugins=grpc:../go/bbh gameserver.proto
protoc -I. -I../include/ -I%GOPATH%/src --go_out=plugins=grpc:../go/bbh client.proto


cmd /k