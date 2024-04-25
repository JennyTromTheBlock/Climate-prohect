import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IonicModule} from '@ionic/angular';
import {FormsModule} from '@angular/forms';
import {HomePage} from './home.page';

import {HomePageRoutingModule} from './home-routing.module';
import {Page1Component} from "./page1/page1.component";
import {Page2Component} from "./page2/page2.component";
import {TopbarComponent} from "./topbar/topbar.component";
import {GraphComponent} from "./graph/graph.component";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HomePageRoutingModule
  ],
  exports: [
    GraphComponent
  ],
  declarations: [HomePage, Page1Component, Page2Component, TopbarComponent, GraphComponent]
})
export class HomePageModule {}
