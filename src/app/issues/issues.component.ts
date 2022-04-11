import { Component, OnInit } from '@angular/core';
import { DataService } from '../data.service';

interface issues{
  id: number;
  date: string;
  affectedSolutions: string;
  text: string;
  eta: string;
  solving: string;
  closing: string;
}

@Component({
  selector: 'app-issues',
  templateUrl: './issues.component.html',
  styleUrls: ['./issues.component.css']
})
export class IssuesComponent implements OnInit {

  currentIssues: issues[] = [];

  constructor(private dataService : DataService) { }

  ngOnInit(): void {
    this.dataService.GetCurrentIssues().subscribe((data: issues[]) => {
      console.log(data);
      this.currentIssues = data;
    })
  }

}
