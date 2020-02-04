// Code generated by protoc-gen-go. DO NOT EDIT.
// source: gameserver.proto

package bbh

import (
	context "context"
	fmt "fmt"
	proto "github.com/golang/protobuf/proto"
	grpc "google.golang.org/grpc"
	codes "google.golang.org/grpc/codes"
	status "google.golang.org/grpc/status"
	math "math"
)

// Reference imports to suppress errors if they are not otherwise used.
var _ = proto.Marshal
var _ = fmt.Errorf
var _ = math.Inf

// This is a compile-time assertion to ensure that this generated file
// is compatible with the proto package it is being compiled against.
// A compilation error at this line likely means your copy of the
// proto package needs to be updated.
const _ = proto.ProtoPackageIsVersion3 // please upgrade the proto package

type PlayerInfoEvent_EventType int32

const (
	PlayerInfoEvent_HEARTBEAT  PlayerInfoEvent_EventType = 0
	PlayerInfoEvent_DISCONNECT PlayerInfoEvent_EventType = 1
)

var PlayerInfoEvent_EventType_name = map[int32]string{
	0: "HEARTBEAT",
	1: "DISCONNECT",
}

var PlayerInfoEvent_EventType_value = map[string]int32{
	"HEARTBEAT":  0,
	"DISCONNECT": 1,
}

func (x PlayerInfoEvent_EventType) String() string {
	return proto.EnumName(PlayerInfoEvent_EventType_name, int32(x))
}

func (PlayerInfoEvent_EventType) EnumDescriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{4, 0}
}

type EventStreamRequest struct {
	// Types that are valid to be assigned to Event:
	//	*EventStreamRequest_Kill
	//	*EventStreamRequest_Earnings
	//	*EventStreamRequest_PlayerInfo
	Event                isEventStreamRequest_Event `protobuf_oneof:"event"`
	XXX_NoUnkeyedLiteral struct{}                   `json:"-"`
	XXX_unrecognized     []byte                     `json:"-"`
	XXX_sizecache        int32                      `json:"-"`
}

func (m *EventStreamRequest) Reset()         { *m = EventStreamRequest{} }
func (m *EventStreamRequest) String() string { return proto.CompactTextString(m) }
func (*EventStreamRequest) ProtoMessage()    {}
func (*EventStreamRequest) Descriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{0}
}

func (m *EventStreamRequest) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_EventStreamRequest.Unmarshal(m, b)
}
func (m *EventStreamRequest) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_EventStreamRequest.Marshal(b, m, deterministic)
}
func (m *EventStreamRequest) XXX_Merge(src proto.Message) {
	xxx_messageInfo_EventStreamRequest.Merge(m, src)
}
func (m *EventStreamRequest) XXX_Size() int {
	return xxx_messageInfo_EventStreamRequest.Size(m)
}
func (m *EventStreamRequest) XXX_DiscardUnknown() {
	xxx_messageInfo_EventStreamRequest.DiscardUnknown(m)
}

var xxx_messageInfo_EventStreamRequest proto.InternalMessageInfo

type isEventStreamRequest_Event interface {
	isEventStreamRequest_Event()
}

type EventStreamRequest_Kill struct {
	Kill *KillEvent `protobuf:"bytes,1,opt,name=kill,proto3,oneof"`
}

type EventStreamRequest_Earnings struct {
	Earnings *EarningsEvent `protobuf:"bytes,2,opt,name=earnings,proto3,oneof"`
}

type EventStreamRequest_PlayerInfo struct {
	PlayerInfo *PlayerInfoEvent `protobuf:"bytes,3,opt,name=player_info,json=playerInfo,proto3,oneof"`
}

func (*EventStreamRequest_Kill) isEventStreamRequest_Event() {}

func (*EventStreamRequest_Earnings) isEventStreamRequest_Event() {}

func (*EventStreamRequest_PlayerInfo) isEventStreamRequest_Event() {}

func (m *EventStreamRequest) GetEvent() isEventStreamRequest_Event {
	if m != nil {
		return m.Event
	}
	return nil
}

func (m *EventStreamRequest) GetKill() *KillEvent {
	if x, ok := m.GetEvent().(*EventStreamRequest_Kill); ok {
		return x.Kill
	}
	return nil
}

func (m *EventStreamRequest) GetEarnings() *EarningsEvent {
	if x, ok := m.GetEvent().(*EventStreamRequest_Earnings); ok {
		return x.Earnings
	}
	return nil
}

func (m *EventStreamRequest) GetPlayerInfo() *PlayerInfoEvent {
	if x, ok := m.GetEvent().(*EventStreamRequest_PlayerInfo); ok {
		return x.PlayerInfo
	}
	return nil
}

// XXX_OneofWrappers is for the internal use of the proto package.
func (*EventStreamRequest) XXX_OneofWrappers() []interface{} {
	return []interface{}{
		(*EventStreamRequest_Kill)(nil),
		(*EventStreamRequest_Earnings)(nil),
		(*EventStreamRequest_PlayerInfo)(nil),
	}
}

type EventStreamResponse struct {
	XXX_NoUnkeyedLiteral struct{} `json:"-"`
	XXX_unrecognized     []byte   `json:"-"`
	XXX_sizecache        int32    `json:"-"`
}

func (m *EventStreamResponse) Reset()         { *m = EventStreamResponse{} }
func (m *EventStreamResponse) String() string { return proto.CompactTextString(m) }
func (*EventStreamResponse) ProtoMessage()    {}
func (*EventStreamResponse) Descriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{1}
}

func (m *EventStreamResponse) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_EventStreamResponse.Unmarshal(m, b)
}
func (m *EventStreamResponse) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_EventStreamResponse.Marshal(b, m, deterministic)
}
func (m *EventStreamResponse) XXX_Merge(src proto.Message) {
	xxx_messageInfo_EventStreamResponse.Merge(m, src)
}
func (m *EventStreamResponse) XXX_Size() int {
	return xxx_messageInfo_EventStreamResponse.Size(m)
}
func (m *EventStreamResponse) XXX_DiscardUnknown() {
	xxx_messageInfo_EventStreamResponse.DiscardUnknown(m)
}

var xxx_messageInfo_EventStreamResponse proto.InternalMessageInfo

type KillEvent struct {
	Killer               string   `protobuf:"bytes,1,opt,name=killer,proto3" json:"killer,omitempty"`
	Victim               string   `protobuf:"bytes,2,opt,name=victim,proto3" json:"victim,omitempty"`
	Cause                string   `protobuf:"bytes,3,opt,name=cause,proto3" json:"cause,omitempty"`
	XXX_NoUnkeyedLiteral struct{} `json:"-"`
	XXX_unrecognized     []byte   `json:"-"`
	XXX_sizecache        int32    `json:"-"`
}

func (m *KillEvent) Reset()         { *m = KillEvent{} }
func (m *KillEvent) String() string { return proto.CompactTextString(m) }
func (*KillEvent) ProtoMessage()    {}
func (*KillEvent) Descriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{2}
}

func (m *KillEvent) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_KillEvent.Unmarshal(m, b)
}
func (m *KillEvent) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_KillEvent.Marshal(b, m, deterministic)
}
func (m *KillEvent) XXX_Merge(src proto.Message) {
	xxx_messageInfo_KillEvent.Merge(m, src)
}
func (m *KillEvent) XXX_Size() int {
	return xxx_messageInfo_KillEvent.Size(m)
}
func (m *KillEvent) XXX_DiscardUnknown() {
	xxx_messageInfo_KillEvent.DiscardUnknown(m)
}

var xxx_messageInfo_KillEvent proto.InternalMessageInfo

func (m *KillEvent) GetKiller() string {
	if m != nil {
		return m.Killer
	}
	return ""
}

func (m *KillEvent) GetVictim() string {
	if m != nil {
		return m.Victim
	}
	return ""
}

func (m *KillEvent) GetCause() string {
	if m != nil {
		return m.Cause
	}
	return ""
}

type EarningsEvent struct {
	User                 string   `protobuf:"bytes,1,opt,name=user,proto3" json:"user,omitempty"`
	Amt                  int64    `protobuf:"varint,2,opt,name=amt,proto3" json:"amt,omitempty"`
	XXX_NoUnkeyedLiteral struct{} `json:"-"`
	XXX_unrecognized     []byte   `json:"-"`
	XXX_sizecache        int32    `json:"-"`
}

func (m *EarningsEvent) Reset()         { *m = EarningsEvent{} }
func (m *EarningsEvent) String() string { return proto.CompactTextString(m) }
func (*EarningsEvent) ProtoMessage()    {}
func (*EarningsEvent) Descriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{3}
}

func (m *EarningsEvent) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_EarningsEvent.Unmarshal(m, b)
}
func (m *EarningsEvent) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_EarningsEvent.Marshal(b, m, deterministic)
}
func (m *EarningsEvent) XXX_Merge(src proto.Message) {
	xxx_messageInfo_EarningsEvent.Merge(m, src)
}
func (m *EarningsEvent) XXX_Size() int {
	return xxx_messageInfo_EarningsEvent.Size(m)
}
func (m *EarningsEvent) XXX_DiscardUnknown() {
	xxx_messageInfo_EarningsEvent.DiscardUnknown(m)
}

var xxx_messageInfo_EarningsEvent proto.InternalMessageInfo

func (m *EarningsEvent) GetUser() string {
	if m != nil {
		return m.User
	}
	return ""
}

func (m *EarningsEvent) GetAmt() int64 {
	if m != nil {
		return m.Amt
	}
	return 0
}

type PlayerInfoEvent struct {
	User                 string                    `protobuf:"bytes,1,opt,name=user,proto3" json:"user,omitempty"`
	EventType            PlayerInfoEvent_EventType `protobuf:"varint,2,opt,name=event_type,json=eventType,proto3,enum=bbh.PlayerInfoEvent_EventType" json:"event_type,omitempty"`
	CurrentBounty        int64                     `protobuf:"varint,3,opt,name=current_bounty,json=currentBounty,proto3" json:"current_bounty,omitempty"`
	CurrentKills         int32                     `protobuf:"varint,4,opt,name=current_kills,json=currentKills,proto3" json:"current_kills,omitempty"`
	CurrentDeaths        int32                     `protobuf:"varint,5,opt,name=current_deaths,json=currentDeaths,proto3" json:"current_deaths,omitempty"`
	XXX_NoUnkeyedLiteral struct{}                  `json:"-"`
	XXX_unrecognized     []byte                    `json:"-"`
	XXX_sizecache        int32                     `json:"-"`
}

func (m *PlayerInfoEvent) Reset()         { *m = PlayerInfoEvent{} }
func (m *PlayerInfoEvent) String() string { return proto.CompactTextString(m) }
func (*PlayerInfoEvent) ProtoMessage()    {}
func (*PlayerInfoEvent) Descriptor() ([]byte, []int) {
	return fileDescriptor_97a199f3261f0fcf, []int{4}
}

func (m *PlayerInfoEvent) XXX_Unmarshal(b []byte) error {
	return xxx_messageInfo_PlayerInfoEvent.Unmarshal(m, b)
}
func (m *PlayerInfoEvent) XXX_Marshal(b []byte, deterministic bool) ([]byte, error) {
	return xxx_messageInfo_PlayerInfoEvent.Marshal(b, m, deterministic)
}
func (m *PlayerInfoEvent) XXX_Merge(src proto.Message) {
	xxx_messageInfo_PlayerInfoEvent.Merge(m, src)
}
func (m *PlayerInfoEvent) XXX_Size() int {
	return xxx_messageInfo_PlayerInfoEvent.Size(m)
}
func (m *PlayerInfoEvent) XXX_DiscardUnknown() {
	xxx_messageInfo_PlayerInfoEvent.DiscardUnknown(m)
}

var xxx_messageInfo_PlayerInfoEvent proto.InternalMessageInfo

func (m *PlayerInfoEvent) GetUser() string {
	if m != nil {
		return m.User
	}
	return ""
}

func (m *PlayerInfoEvent) GetEventType() PlayerInfoEvent_EventType {
	if m != nil {
		return m.EventType
	}
	return PlayerInfoEvent_HEARTBEAT
}

func (m *PlayerInfoEvent) GetCurrentBounty() int64 {
	if m != nil {
		return m.CurrentBounty
	}
	return 0
}

func (m *PlayerInfoEvent) GetCurrentKills() int32 {
	if m != nil {
		return m.CurrentKills
	}
	return 0
}

func (m *PlayerInfoEvent) GetCurrentDeaths() int32 {
	if m != nil {
		return m.CurrentDeaths
	}
	return 0
}

func init() {
	proto.RegisterEnum("bbh.PlayerInfoEvent_EventType", PlayerInfoEvent_EventType_name, PlayerInfoEvent_EventType_value)
	proto.RegisterType((*EventStreamRequest)(nil), "bbh.EventStreamRequest")
	proto.RegisterType((*EventStreamResponse)(nil), "bbh.EventStreamResponse")
	proto.RegisterType((*KillEvent)(nil), "bbh.KillEvent")
	proto.RegisterType((*EarningsEvent)(nil), "bbh.EarningsEvent")
	proto.RegisterType((*PlayerInfoEvent)(nil), "bbh.PlayerInfoEvent")
}

func init() { proto.RegisterFile("gameserver.proto", fileDescriptor_97a199f3261f0fcf) }

var fileDescriptor_97a199f3261f0fcf = []byte{
	// 415 bytes of a gzipped FileDescriptorProto
	0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xff, 0x6c, 0x92, 0xcf, 0x6e, 0xd3, 0x40,
	0x10, 0xc6, 0xeb, 0x3a, 0x29, 0x78, 0x42, 0x42, 0x34, 0x14, 0xb0, 0x38, 0xa0, 0xca, 0x80, 0x14,
	0x71, 0x88, 0x50, 0x10, 0xe2, 0xc4, 0xa1, 0x6e, 0x2d, 0x5a, 0x21, 0x15, 0xba, 0xf1, 0x3d, 0x5a,
	0x9b, 0x69, 0x63, 0xe1, 0x7f, 0xec, 0xae, 0x2d, 0xf9, 0xcc, 0xfb, 0xf0, 0x8c, 0xc8, 0x13, 0xdb,
	0x6a, 0x69, 0x2e, 0xd6, 0xcc, 0xcf, 0xdf, 0x8c, 0xbe, 0x99, 0x1d, 0x98, 0xdf, 0xca, 0x8c, 0x34,
	0xa9, 0x9a, 0xd4, 0xb2, 0x54, 0x85, 0x29, 0xd0, 0x8e, 0xa2, 0xad, 0xf7, 0xd7, 0x02, 0x0c, 0x6a,
	0xca, 0xcd, 0xda, 0x28, 0x92, 0x99, 0xa0, 0xdf, 0x15, 0x69, 0x83, 0x6f, 0x61, 0xf4, 0x2b, 0x49,
	0x53, 0xd7, 0x3a, 0xb1, 0x16, 0x93, 0xd5, 0x6c, 0x19, 0x45, 0xdb, 0xe5, 0xb7, 0x24, 0x4d, 0x59,
	0x7a, 0x71, 0x20, 0xf8, 0x2f, 0x7e, 0x80, 0xc7, 0x24, 0x55, 0x9e, 0xe4, 0xb7, 0xda, 0x3d, 0x64,
	0x25, 0xb2, 0x32, 0xe8, 0x60, 0xaf, 0x1e, 0x54, 0xf8, 0x19, 0x26, 0x65, 0x2a, 0x1b, 0x52, 0x9b,
	0x24, 0xbf, 0x29, 0x5c, 0x9b, 0x8b, 0x8e, 0xb9, 0xe8, 0x07, 0xf3, 0xcb, 0xfc, 0xa6, 0xe8, 0xcb,
	0xa0, 0x1c, 0x90, 0xff, 0x08, 0xc6, 0xd4, 0x62, 0xef, 0x39, 0x3c, 0xbb, 0xe7, 0x57, 0x97, 0x45,
	0xae, 0xc9, 0xbb, 0x06, 0x67, 0xf0, 0x87, 0x2f, 0xe0, 0xa8, 0xf5, 0x47, 0x8a, 0xfd, 0x3b, 0xa2,
	0xcb, 0x5a, 0x5e, 0x27, 0xb1, 0x49, 0x32, 0x76, 0xeb, 0x88, 0x2e, 0xc3, 0x63, 0x18, 0xc7, 0xb2,
	0xd2, 0xc4, 0x7e, 0x1c, 0xb1, 0x4b, 0xbc, 0x4f, 0x30, 0xbd, 0x37, 0x08, 0x22, 0x8c, 0x2a, 0x3d,
	0x34, 0xe5, 0x18, 0xe7, 0x60, 0xcb, 0xcc, 0x70, 0x3f, 0x5b, 0xb4, 0xa1, 0xf7, 0xe7, 0x10, 0x9e,
	0xfe, 0x37, 0xcb, 0xde, 0xca, 0x2f, 0x00, 0x3c, 0xd1, 0xc6, 0x34, 0x25, 0x71, 0x83, 0xd9, 0xea,
	0xf5, 0xbe, 0x4d, 0x2c, 0xf9, 0x1b, 0x36, 0x25, 0x09, 0x87, 0xfa, 0x10, 0xdf, 0xc1, 0x2c, 0xae,
	0x94, 0x6a, 0x1b, 0x44, 0x45, 0x95, 0x9b, 0x86, 0xcd, 0xdb, 0x62, 0xda, 0x51, 0x9f, 0x21, 0xbe,
	0x81, 0x1e, 0x6c, 0xda, 0x25, 0x68, 0x77, 0x74, 0x62, 0x2d, 0xc6, 0xe2, 0x49, 0x07, 0xdb, 0x9d,
	0xe9, 0xbb, 0xbd, 0x7e, 0x92, 0x34, 0x5b, 0xed, 0x8e, 0x59, 0xd5, 0x97, 0x9e, 0x33, 0xf4, 0xde,
	0x83, 0x33, 0x58, 0xc1, 0x29, 0x38, 0x17, 0xc1, 0xa9, 0x08, 0xfd, 0xe0, 0x34, 0x9c, 0x1f, 0xe0,
	0x0c, 0xe0, 0xfc, 0x72, 0x7d, 0xf6, 0xfd, 0xea, 0x2a, 0x38, 0x0b, 0xe7, 0xd6, 0xea, 0x1a, 0x26,
	0x5f, 0x65, 0x46, 0x6b, 0x52, 0x75, 0x12, 0x13, 0xfa, 0x30, 0xb9, 0xf3, 0x6a, 0xf8, 0x72, 0x77,
	0x26, 0x0f, 0xee, 0xee, 0x95, 0xfb, 0xf0, 0xc7, 0xee, 0x81, 0x17, 0x56, 0x74, 0xc4, 0x67, 0xfb,
	0xf1, 0x5f, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xd3, 0x3e, 0xe4, 0xca, 0x02, 0x00, 0x00,
}

// Reference imports to suppress errors if they are not otherwise used.
var _ context.Context
var _ grpc.ClientConnInterface

// This is a compile-time assertion to ensure that this generated file
// is compatible with the grpc package it is being compiled against.
const _ = grpc.SupportPackageIsVersion6

// GameServiceClient is the client API for GameService service.
//
// For semantics around ctx use and closing/ending streaming RPCs, please refer to https://godoc.org/google.golang.org/grpc#ClientConn.NewStream.
type GameServiceClient interface {
	EventStream(ctx context.Context, opts ...grpc.CallOption) (GameService_EventStreamClient, error)
}

type gameServiceClient struct {
	cc grpc.ClientConnInterface
}

func NewGameServiceClient(cc grpc.ClientConnInterface) GameServiceClient {
	return &gameServiceClient{cc}
}

func (c *gameServiceClient) EventStream(ctx context.Context, opts ...grpc.CallOption) (GameService_EventStreamClient, error) {
	stream, err := c.cc.NewStream(ctx, &_GameService_serviceDesc.Streams[0], "/bbh.GameService/EventStream", opts...)
	if err != nil {
		return nil, err
	}
	x := &gameServiceEventStreamClient{stream}
	return x, nil
}

type GameService_EventStreamClient interface {
	Send(*EventStreamRequest) error
	CloseAndRecv() (*EventStreamResponse, error)
	grpc.ClientStream
}

type gameServiceEventStreamClient struct {
	grpc.ClientStream
}

func (x *gameServiceEventStreamClient) Send(m *EventStreamRequest) error {
	return x.ClientStream.SendMsg(m)
}

func (x *gameServiceEventStreamClient) CloseAndRecv() (*EventStreamResponse, error) {
	if err := x.ClientStream.CloseSend(); err != nil {
		return nil, err
	}
	m := new(EventStreamResponse)
	if err := x.ClientStream.RecvMsg(m); err != nil {
		return nil, err
	}
	return m, nil
}

// GameServiceServer is the server API for GameService service.
type GameServiceServer interface {
	EventStream(GameService_EventStreamServer) error
}

// UnimplementedGameServiceServer can be embedded to have forward compatible implementations.
type UnimplementedGameServiceServer struct {
}

func (*UnimplementedGameServiceServer) EventStream(srv GameService_EventStreamServer) error {
	return status.Errorf(codes.Unimplemented, "method EventStream not implemented")
}

func RegisterGameServiceServer(s *grpc.Server, srv GameServiceServer) {
	s.RegisterService(&_GameService_serviceDesc, srv)
}

func _GameService_EventStream_Handler(srv interface{}, stream grpc.ServerStream) error {
	return srv.(GameServiceServer).EventStream(&gameServiceEventStreamServer{stream})
}

type GameService_EventStreamServer interface {
	SendAndClose(*EventStreamResponse) error
	Recv() (*EventStreamRequest, error)
	grpc.ServerStream
}

type gameServiceEventStreamServer struct {
	grpc.ServerStream
}

func (x *gameServiceEventStreamServer) SendAndClose(m *EventStreamResponse) error {
	return x.ServerStream.SendMsg(m)
}

func (x *gameServiceEventStreamServer) Recv() (*EventStreamRequest, error) {
	m := new(EventStreamRequest)
	if err := x.ServerStream.RecvMsg(m); err != nil {
		return nil, err
	}
	return m, nil
}

var _GameService_serviceDesc = grpc.ServiceDesc{
	ServiceName: "bbh.GameService",
	HandlerType: (*GameServiceServer)(nil),
	Methods:     []grpc.MethodDesc{},
	Streams: []grpc.StreamDesc{
		{
			StreamName:    "EventStream",
			Handler:       _GameService_EventStream_Handler,
			ClientStreams: true,
		},
	},
	Metadata: "gameserver.proto",
}
