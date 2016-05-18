# TODO

# Wishes

- [X] Get rid of dynamics to support platforms that don't have JIT and for  (Moving over to JObjects)
- [X] Handle errors proactively (Some methods are covered, e.g. login)
- [ ] An API similar to Margie Bot for Slack (e.g. inline lambda responders, standalone Responders, etc.)
- [ ] Programmatic bot modifications (e.g. change avatar, etc.)
- [X] Continuous integration testing on a real Rocket.Chat server (need a way to verify interactions)
- [X] Get test coverage
- [X] A way to unsubscribe
- [X] Figure out why tests are stuck when running in parallel. - It runs correctly locally from the cli. I almost looks like the tests don't even start
- [X] Pull user info from env vars for tests
- [ ] Handle clears on subs results
- [X] Create a converter for rocket messages
- [ ] Check mono support later when Mono isn't completely broken for 4.6.1 and C# 6.0 nuget. 
- [X] Get a different RC server, DO is too slow (Maxing CPU). 
- [ ] API docs.

# Driver Support

## DDP

- [X] Ping/Pong
- [X] Subscribe and get responses
- [X] Unsubscribe
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
- [X] Search for messages in room
- [X] Receive messages in room
- [X] Receive messages in all rooms
- [X] Load history
- [X] Send reactions
- [X] Receieve reactions
- [X] Pinning
- [X] Unpinning

## Client

- [X] Login with email
- [X] Login with username
- [X] Login with LDAP
- [X] Resume session
- [ ] Login with intergration
- [ ] Sign-up
- [ ] Change current user info
- [X] Log out all other sessions
- [X] Reconnects
- [X] Ping

## Users

- [X] Get full user information
- [ ] Update user information
- [ ] Create user
- [ ] Delete user
- [ ] Search by username
- [ ] Log out other sessions
- [X] Set avatar to URL
- [X] Set avatar to uploaded image
- [X] Reset avatar

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
- [X] List

#### Private Groups

- [ ] Create
- [ ] Update
- [ ] Delete
- [ ] Hide
- [X] List

#### Private Messages

- [X] Create
- [ ] Update
- [ ] Delete
- [ ] Hide
- [X] List

## File Management

- [ ] File download in room
- [ ] File search in room
- [ ] File upload in room