import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {filter, map, Subject, switchMap, takeUntil} from "rxjs";
import {Device, DeviceInRoom, Room} from "../../../../models/Entities";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {RoomService} from "./room.service";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../../models/ClientWantsToGetDeviceIdsForRoomDto";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
})
export class RoomComponent  implements OnInit {
  idFromRoute: number | undefined;
  private unsubscribe$ = new Subject<void>();

  room!: Room

  constructor(private activatedRoute: ActivatedRoute,
              private roomService: RoomService,
              private ws: WebSocketConnectionService) {
  }

  ngOnInit() {

    this.getRoomFromRoute();//todo skal bruges til at loade room info later
    this.subscribeToRoomDevice();//todo skal ændres til allrooms.
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }

  //todo load a list of all deviceId into all rooms observable
  subscribeToRoomDevice() {
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          if (roomRecord[this.idFromRoute!].DeviceIds == null){
            this.ws.socketConnection.sendDto(new ClientWantsToGetDeviceIdsForRoomDto({ RoomId: this.idFromRoute}))
          }
          const selectedRoom = roomRecord[this.idFromRoute!];
          //checks if any changes in room from server and updates room and devices
            this.room = selectedRoom;
          }
      });
  }

}
