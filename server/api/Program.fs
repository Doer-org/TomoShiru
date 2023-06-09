﻿module Program =
    open System
    open Falco
    open Falco.Routing
    open Falco.HostBuilder
    open Microsoft.Extensions.DependencyInjection
    open MySql.Data.MySqlClient

    open Database

    [<EntryPoint>]
    let main _ =
        let env =
            let ENVIRONMENT = Environment.GetEnvironmentVariable("ENVIRONMENT")
            let DB_HOST = Environment.GetEnvironmentVariable("DB_HOST")
            let DB_USER = Environment.GetEnvironmentVariable("DB_USER")
            let DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD")
            let DB_DATABASE = Environment.GetEnvironmentVariable("DB_DATABASE")
            let CLIENT_URL = Environment.GetEnvironmentVariable("CLIENT_URL")

            let AUTH0_DOMAIN =
                Environment.GetEnvironmentVariable("AUTH0_DOMAIN")

            let AUTH0_AUDIENCE =
                Environment.GetEnvironmentVariable("AUTH0_AUDIENCE")

            let AUTH0_JWKS = Environment.GetEnvironmentVariable("AUTH0_JWKS")

            { new Env.IEnv with
                member _.ENVIRONMENT = ENVIRONMENT
                member _.DB_HOST = DB_HOST
                member _.DB_USER = DB_USER
                member _.DB_PASSWORD = DB_PASSWORD
                member _.DB_DATABASE = DB_DATABASE
                member _.CLIENT_URL = CLIENT_URL
                member _.AUTH0_DOMAIN = AUTH0_DOMAIN
                member _.AUTH0_AUDIENCE = AUTH0_AUDIENCE
                member _.AUTH0_JWKS = AUTH0_JWKS }

        let storeService =
            let connectionString =
                $"Server={env.DB_HOST};Port=3306;Database={env.DB_DATABASE};user={env.DB_USER};password={env.DB_PASSWORD}"

            let dbFactory =
                { new Database.IDbConnectionFactory with
                    member _.CreateConnection() =
                        let conn = new MySqlConnection(connectionString)
                        conn.Open()
                        conn }

            let store =
                { new Store.IStore with

                    member _.createAccount(account: Domain.User.Account) =
                        let conn = dbFactory.CreateConnection()
                        Database.Account.create conn account

                    member _.createUser(user: Domain.User) =
                        let conn = dbFactory.CreateConnection()
                        let _ = Database.User.save conn user
                        Ok user

                    member _.followUser(follow: Domain.User.Follow) =
                        let conn = dbFactory.CreateConnection()
                        Database.Follow.follow conn follow

                    member _.unfollowUser(follow: Domain.User.Follow) =
                        let conn = dbFactory.CreateConnection()
                        Database.Follow.unfollow conn follow

                    member _.updateReaction(reaction: Domain.Profile.Reaction) =
                        let conn = dbFactory.CreateConnection()
                        Database.Reaction.create conn reaction

                    member _.updateFavoriteMusic
                        (music: Domain.Profile.Music.FavoriteMusic)
                        =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteMusic.update conn music

                    member _.updateFavoriteArtist
                        (artist: Domain.Profile.Music.FavoriteArtist)
                        =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteArtist.update conn artist

                    member _.updateRamenProfile
                        (ramenya: Domain.Profile.Ramen.FavoriteRamenya)
                        =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteRamenya.update conn ramenya

                    member _.updateLog(log: Domain.User.Log) =
                        let conn = dbFactory.CreateConnection()
                        Database.Log.update conn log

                    member _.createChangeLog(log: Domain.Profile.ChangeLog) =
                        let conn = dbFactory.CreateConnection()
                        Database.ProfileChangeLog.create conn log

                    member _.getUser(user_id: string) =
                        let conn = dbFactory.CreateConnection()
                        let user = Database.User.get conn user_id
                        user

                    member _.getLog(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.Log.getByUserID conn user_id

                    member _.getAccount(account: Domain.User.Account) =
                        let conn = dbFactory.CreateConnection()
                        Database.Account.getBySub conn account.sub

                    member _.getAccountBySub(sub: Domain.sub) =
                        let conn = dbFactory.CreateConnection()
                        Database.Account.getBySub conn sub

                    member _.getAllUsers() =
                        let conn = dbFactory.CreateConnection()
                        let users = Database.User.getAll conn |> Seq.toList
                        Ok users

                    member _.getFollowingUsers(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.Follow.getFollowing conn user_id

                    member _.getFollowers(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.Follow.getFollowers conn user_id

                    member _.getTimeline(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.ProfileChangeLog.getByUserID conn user_id

                    member _.getAllTimeline() =
                        let conn = dbFactory.CreateConnection()
                        Database.ProfileChangeLog.getAll conn

                    // member _.getProfile(user_id: Domain.UserID) =
                    //     let conn = dbFactory.CreateConnection()
                    //     Error "未実装" // TODO: 未実装

                    member _.getReaction(user_id_to: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.Reaction.getByUserID conn user_id_to

                    member _.getRamenProfile(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteRamenya.getByUserID conn user_id

                    member _.getFavoriteMusicProfile(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteMusic.getByUserID conn user_id

                    member _.getFavoriteArtistsProfile(user_id: Domain.UserID) =
                        let conn = dbFactory.CreateConnection()
                        Database.FavoriteArtist.getByUserID conn user_id

                }

            fun (svc: IServiceCollection) ->
                svc.AddSingleton<Store.IStore>(fun _ -> store)

        let validate permissions handler : HttpHandler =
            if (env.ENVIRONMENT = "test") then
                handler
            else
                Auth.validate
                    (env.AUTH0_JWKS, env.AUTH0_AUDIENCE, env.AUTH0_AUDIENCE)
                    permissions
                    Rest.Handlers.Error.index
                    handler


        webHost [||] {
            add_service storeService

            use_cors "CorsPolicy" (fun options ->
                options.AddPolicy(
                    "CorsPolicy",
                    fun builder ->
                        builder.AllowAnyHeader() |> ignore
                        builder.AllowAnyMethod() |> ignore

                        builder.WithOrigins(env.CLIENT_URL).AllowCredentials()
                        |> ignore
                ))

            endpoints
                [ get
                      "/"
                      (Response.ofPlainText $"Hello F# World! {env.ENVIRONMENT}")
                  post
                      "/graphql"
                      (GraphQL.Handler.handleGraphQL
                          (env.ENVIRONMENT = "test")
                          (env.AUTH0_JWKS, env.AUTH0_DOMAIN, env.AUTH0_AUDIENCE))
                  get "/users" (validate [] Rest.Handlers.Users.index)
                  get "/users/{id}" (validate [] Rest.Handlers.Users.read)
                  post "/users" (validate [] Rest.Handlers.Users.create) ]
        }

        0
