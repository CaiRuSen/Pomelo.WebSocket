# Pomelo.WebSocket
基于 .Net core 3.1 和 redis 的 websocket 通讯服务

该服务单纯的实现了 ws 的基本功能，实现消息的推送（发送消息给目标用户ws连接端）
并提供额外的 api 功能 给 web服务 去使用，比如 在线的用户数量（AllOnlineUser接口） 和 某个用户是否在线（UserIsOnline接口），顺便提供消息的测试发送接口（Test_ClientSendMsg接口）（该接口用于测试 ws 服务是否正常运行，可用 http://www.websocket-test.com 来进行连接ws测试看看是否能连接正常并收到消息）

使用步骤：
1.配置 appsettings.json 文件（Auth，Redis 和 WebSocket.Host 的相关配置）
2.获取 ws 连接（GetWSLink接口）
3.客户端进行 ws 连接，并发送心跳包保持连接
4.可用服务提供的发送消息（Test_ClientSendMsg接口）

具体说明可查看 https://blog.muops.com/?p=1104
