import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Dating App';

  // users will be stred in this variable
  users: any;

  // this is DI like in .net app. 
  // Im injecting private field of type HttpClient
  constructor(private http: HttpClient) {}

  // So on app init this function will this 'getUsers' method. 
  ngOnInit() {
    this.getUsers();
  }

  //So on app init this function go and get users from the api.
  // Subscribe has two parts, response when it succeed and error when it fails. Both are callback functions.
  // response is when the api call succeded and it will store users in users variable.
  // on error I will just log it to console.
  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error)
    });
  }
}

