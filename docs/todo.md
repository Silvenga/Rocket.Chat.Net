# TODO

# Wishes

- [ ] Get rid of dynamics to support platforms that don't have JIT and for  (Moving over to JObjects)
- [ ] Handle errors proactively (Some methods are covered, e.g. login)
- [ ] An api similar to Margie Bot for Slack (e.g. inline lambda responders, standalone Responders, etc.)
- [ ] Programmatic bot modifications (e.g. change avatar, etc.)
- [X] Continuous integration testing on a real Rocket.Chat server (need a way to verify interactions)
- [ ] Get test converage
- [ ] A way to unsub
- [ ] Figure out why tests are stuck when running in parallel. - It runs correctly locally from the cli. I almost looks like the tests don't even start
- [X] Pull user info from env vars for tests
- [ ] Handle clears on subs results
- [X] Create a converter for rocket messages
- [ ] Check mono support later when Mono isn't completely broken for 4.6.1 and C# 6.0 nuget. 
- [X] Get a different RC server, DO is too slow (Maxing CPU). 

# Driver Support

## DDP

- [X] Ping/Pong
- [X] Subscribe and get responses
- [ ] Unsubscribe
- [X] Call server method and get response
- [X] Streaming collections
- [X] Reconnecting
- [X] Protocol versions - using version 1. 
- [X] Start session
- [X] Resume session

## Messaging

- [X] Send basic message to room
- [ ] Update basic message in room
- [X] Delete message in room
- [ ] Search for messages in room
- [X] Receive messages in room
- [X] Receive messages in all rooms
- [X] Load history
- [ ] File download in room
- [ ] File search in room
- [ ] File upload in room
- [X] Send reactions
- [X] Receieve reactions

## Client

- [X] Login with email
- [X] Login with username
- [X] Login with LDAP
- [X] Resume session
- [ ] Login with intergration
- [ ] Sign-up
- [ ] Change current user info
- [X] Log out all other sessions

## Users

- [X] Get full user information
- [ ] Update user information
- [ ] Create user
- [ ] Delete user
- [ ] Search by username

## Status

- [ ] Away status
- [ ] Online status
- [ ] Typing started
- [ ] Typing stopped

## Rooms

#### Channels

- [X] Create
- [ ] Update
- [X] Delete
- [X] Hide
- [ ] List

#### Private Groups

- [ ] Create
- [ ] Update
- [ ] Delete
- [ ] Hide
- [ ] List

#### Private Messages

- [X] Create
- [ ] Update
- [ ] Delete
- [ ] Hide
- [ ] List