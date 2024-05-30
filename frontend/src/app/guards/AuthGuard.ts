import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import {WebSocketConnectionService} from "../web-socket-connection.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor( private router: Router, private ws: WebSocketConnectionService) {}

  canActivate(): boolean {
    // Tjek om brugeren har en gyldig JWT
    if (this.isLoggedIn()) {
      console.log("is logdged in")
      return true; // Tillad adgang til ruten, hvis brugeren er logget ind

    } else {
      // Hvis brugeren ikke er logget ind, omdiriger til login-siden
      this.router.navigate(['/auth/login']);
      console.log("is not logged in")
      return false; // Bloker adgang til ruten
    }
  }

  //todo temporary logged in method (should be some more)
  isLoggedIn(): boolean {
    const currentJwt = this.ws.jwtSubject.getValue();
    return currentJwt !== undefined && currentJwt !== '';

  }
}
