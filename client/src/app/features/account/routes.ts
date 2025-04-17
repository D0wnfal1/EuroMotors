import { Route } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { CustomerCarComponent } from './customer-car/customer-car.component';

export const accountRourtes: Route[] = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'customer-car', component: CustomerCarComponent },
];
