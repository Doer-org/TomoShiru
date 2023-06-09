﻿module GraphQL.Schema.Mutation

open System
open FSharp.Data.GraphQL.Types

open GraphQL.Schema.Types
open Domain

module private MutationCommand =
    let createUser = "createUser"
    let updateFollow = "updateFollow"
    let updateReaction = "updateReaction"
    let postTimeLine = "postTimeLine"
    let updateFavoriteMusic = "updateFavoriteMusic"
    let updateFavoriteArtist = "updateFavoriteArtist"
    let updateRamenProfile = "updateRamenProfile"


let private createUser
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    Define.Field(
        MutationCommand.createUser,
        Nullable UserDataType,
        "create user",
        fun _ _ ->
            let sub =
                token
                |> function
                    | Some token -> token.sub
                    | None ->
                        if isTest then
                            System.Guid.NewGuid() |> string
                        else
                            failwith "no sub"

            let user: User =
                let id = System.Guid.NewGuid() |> string

                match token with
                | None ->
                    if not isTest then
                        failwith "token empty (check env)"
                    else
                        { user_id = id
                          user_name =
                            $"token empty (created:{DateTimeOffset.Now})"
                          image_url = "" }
                | Some token ->
                    { user_id = id
                      user_name = token.name
                      image_url = token.picture }

            user
            |> Domain.createUser
                (Utils.checkSub store)
                (store.createAccount)
                (store.createUser)
                sub
            |> function
                | Error e -> failwith e
                | Ok user -> Some user
    )

let private createReaction
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id_from = "user_id_from"
           user_id_to = "user_id_to"
           kind = "kind" |}

    Define.Field(
        MutationCommand.updateReaction,
        Nullable ProfileReactionType,
        "update reaction",
        [ Define.Input(args.user_id_from, String)
          Define.Input(args.user_id_to, String)
          Define.Input(args.kind, String) ],
        fun ctx _ ->
            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let reaction: Profile.Reaction =
                { reaction_id = System.Guid.NewGuid() |> string
                  user_id_from = ctx.Arg args.user_id_from
                  user_id_to = ctx.Arg args.user_id_to
                  kind = ctx.Arg args.kind
                  timestamp = DateTimeOffset.Now }

            let changeLog: Profile.ChangeLog =
                { user_id = reaction.user_id_from
                  summary = "reaction!!!" + string DateTimeOffset.Now
                  timestamp = DateTimeOffset.Now }

            let account: User.Account =
                { sub = sub
                  user_id = reaction.user_id_from }

            let result =
                Domain.updateReaction
                    (store.updateReaction)
                    account
                    (Utils.accountValidation isTest store)
                    reaction
                    changeLog

            result
            |> function
                | Error e -> failwith e
                | Ok r -> Some r
    )

let private updateFollow
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id_from = "user_id_from"
           user_id_to = "user_id_to"
           isFollow = "isFollow" |}

    Define.Field(
        MutationCommand.updateFollow,
        Nullable UserFollowType,
        "update follow",
        [ Define.Input(args.user_id_from, String)
          Define.Input(args.user_id_to, String)
          Define.Input(args.isFollow, Boolean) ],
        fun ctx _ ->
            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let follow: User.Follow =
                { user_id_from = ctx.Arg args.user_id_from
                  user_id_to = ctx.Arg args.user_id_to }

            let isFollow: bool = ctx.Arg args.isFollow

            let account: User.Account =
                { sub = sub
                  user_id = follow.user_id_from }

            let result =
                Domain.updateFollowUser
                    (store.followUser)
                    (store.unfollowUser)
                    account
                    (Utils.accountValidation isTest store)
                    {| follow = follow
                       isFollow = isFollow |}

            result
            |> function
                | Error e -> failwith e
                | Ok follow -> Some follow
    )

let private postTimeLine
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id = "user_id"
           summary = "summary" |}

    Define.Field(
        MutationCommand.postTimeLine,
        Nullable ProfileChangeLogType,
        "post timeline",
        [ Define.Input(args.user_id, String)
          Define.Input(args.summary, String) ],
        fun ctx _ ->
            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let log: Profile.ChangeLog =
                { user_id = ctx.Arg args.user_id
                  summary = ctx.Arg args.summary
                  timestamp = DateTimeOffset.Now }

            let account: User.Account = { sub = sub; user_id = log.user_id }

            let result =
                Domain.postTimeLine
                    (store.createChangeLog)
                    account
                    (Utils.accountValidation isTest store)
                    log

            result
            |> function
                | Error e -> failwith e
                | Ok log -> Some log
    )

let private updateFavoriteArtist
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id = "user_id"
           rank = "rank"
           artist = "artist" |}

    Define.Field(
        MutationCommand.updateFavoriteArtist,
        Nullable ProfileMusicFavoriteArtistType,
        "update favotrite artists",
        [ Define.Input(args.user_id, String)
          Define.Input(args.artist, String)
          Define.Input(args.rank, Int) ],
        fun ctx _ ->

            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let artist: Profile.Music.FavoriteArtist =
                { artist = ctx.Arg args.artist
                  user_id = ctx.Arg args.user_id
                  rank = ctx.Arg args.rank
                  timestamp = DateTimeOffset.Now }

            let account: User.Account = { sub = sub; user_id = artist.user_id }

            artist
            |> Domain.updateFavoriteArtist
                store.updateFavoriteArtist
                account
                (Utils.accountValidation isTest store)
            |> function
                | Error e -> failwith e
                | Ok follow -> Some follow
    )

let private updateFavoriteMusic
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id = "user_id"
           rank = "rank"
           music = "music" |}

    Define.Field(
        MutationCommand.updateFavoriteMusic,
        Nullable ProfileMusicFavoriteMusicType,
        "update favorite music",
        [ Define.Input(args.user_id, String)
          Define.Input(args.music, String)
          Define.Input(args.rank, Int) ],
        fun ctx _ ->

            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let music: Profile.Music.FavoriteMusic =
                { music = ctx.Arg args.music
                  user_id = ctx.Arg args.user_id
                  rank = ctx.Arg args.rank
                  timestamp = DateTimeOffset.Now }

            let account: User.Account = { sub = sub; user_id = music.user_id }

            music
            |> Domain.updateFavoriteMusic
                store.updateFavoriteMusic
                account
                (Utils.accountValidation isTest store)
            |> function
                | Error e -> failwith e
                | Ok follow -> Some follow
    )

let private updateRamenProfile
    (isTest: bool)
    (token: Domain.Token option)
    (store: Store.IStore)
    =
    let args =
        {| user_id = "user_id"
           ramenya = "ramenya"
           rank = "rank" |}

    Define.Field(
        MutationCommand.updateRamenProfile,
        Nullable ProfileRamenFavoriteRamenyaType,
        "update ramen profile",
        [ Define.Input(args.user_id, String)
          Define.Input(args.ramenya, String)
          Define.Input(args.rank, Int) ],
        fun ctx _ ->

            let sub =
                token
                |> function
                    | None -> if isTest then null else failwith "no sub"
                    | Some token -> token.sub

            let ramen: Profile.Ramen.FavoriteRamenya =
                { rank = ctx.Arg args.rank
                  user_id = ctx.Arg args.user_id
                  ramenya = ctx.Arg args.ramenya
                  timestamp = DateTimeOffset.Now }

            let account: User.Account = { sub = sub; user_id = ramen.user_id }

            ramen
            |> Domain.updateFavoriteRamen
                store.updateRamenProfile
                account
                (Utils.accountValidation isTest store)
            // (fun _ ->
            //     let log: Domain.Profile.ChangeLog =
            //         { user_id = ramen.user_id
            //           summary = "update ramen profile"
            //           timestamp = DateTimeOffset.Now }

            //     store.createChangeLog log
            //     |> function
            //         | Error e -> printfn "%A" e
            //         | Ok _ -> ())
            |> function
                | Error e -> failwith e
                | Ok follow -> Some follow
    )

let Mutation (isTest: bool) (token: Domain.Token option) (store: Store.IStore) =
    Define.Object<Root>(
        name = "Mutation",
        fields =
            [ createUser isTest token store
              createReaction isTest token store
              postTimeLine isTest token store
              updateFollow isTest token store
              updateFavoriteMusic isTest token store
              updateFavoriteArtist isTest token store
              updateRamenProfile isTest token store ]
    )
