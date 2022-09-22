import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-dateinput',
  templateUrl: './dateinput.component.html',
  styleUrls: ['./dateinput.component.css']
})
export class DateinputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() maxDate: Date;
  bsconfig: Partial<BsDatepickerConfig>;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsconfig = {
      containerClass: 'theme-default',
      dateInputFormat: 'DD MMMM YYYY',
    }
   }
  writeValue(obj: any): void {
    
  }
  registerOnChange(fn: any): void {
    
  }
  registerOnTouched(fn: any): void {
    
  }

  ngOnInit(): void {
  }

}
