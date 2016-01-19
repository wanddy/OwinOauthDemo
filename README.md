# OwinOauthDemo
-------------------------------------------------------------------
本demo目前只实现了Oauth的client、password授权模式，暂不考虑其他模式
-------------------------------------------------------------------
项目计划：<br>
1、在项目中搜索 “待续工作” 将其完成 &nbsp;&nbsp;&nbsp;完成<br> 
1.1、refresh token 的持久化              &nbsp;&nbsp;&nbsp;完成<br>
2、将demo的服务端整合到FoodShop项目中 <br>
3、制作本demo的 实现文档、使用指导文档<br>
4、探索MvcClient的实现（考察DotNetOpenAuth的可行性）&nbsp;&nbsp;&nbsp;完成<br>

-------------------------------------------------------------------------------
引入本demo的步骤：<br>
----------------------------------------------------------------------------
1）用Visual Studio 2013/2015创建一个Web API项目，VS会生成一堆OAuth相关代码。<br>
2）添加AuthData.db 到 App_Data<br>
3）引入程序集Dapper.dll、SqliteProvider、Sqlinq.dll、Sqlinq.Dapper.dll<br>
4）拷贝AuthorizationServer文件夹到项目中，<br>
5）if (!UserService.Login(context.UserName, context.Password)) 处会报错，这是在验证uid、pwd是否正确，根据环境实现此处自定义的代码。<br>
6）修改App_Start/Startup.Auth.cs 的代码<br>
7）修改Controller目录下的三个测试Controller的代码，以便进行授权服务器的测试<br>
8）准备 测试客户端 https://github.com/goldenKeyHu/MvcClient，注意：要根据自己服务器的端口修改baseAddress<br>

