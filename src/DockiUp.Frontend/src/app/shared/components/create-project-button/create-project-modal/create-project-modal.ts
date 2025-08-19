import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { MatDialogContent, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { ContainerOriginType, ContainerUpdateMethod } from '../../../../api';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

export interface CreateContainerRequest {
  // Project Information (Step 1)
  projectName: string;
  description?: string;

  // Project Origin (Step 2)
  originType: ContainerOriginType;
  gitUrl?: string;
  composeContent?: string;
  path?: string;

  // Update Method (Step 3)
  updateMethod: ContainerUpdateMethod;
  webhookUrl?: string;
  periodicInterval?: number;
}

@Component({
  selector: 'app-create-project-modal',
  imports: [
    MatDialogModule,
    MatDialogContent,
    MatProgressBarModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatRadioModule,
    MatAutocompleteModule
  ],
  templateUrl: './create-project-modal.html',
  styleUrl: './create-project-modal.scss'
})
export class CreateProjectModal implements OnInit {
  public isloading = signal(false);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreateProjectModal>);

  // Enums for template
  readonly ContainerOriginType = ContainerOriginType;
  readonly ContainerUpdateMethod = ContainerUpdateMethod;

  // Autocomplete data
  readonly paths: string[] = [
    '/home/user/project1',
    '/var/www/app',
    '/opt/docker/project2',
    '/srv/data/project3',
    '/usr/local/share/project4'
  ];

  pathValue = signal('');
  filteredPaths = computed(() =>
    this.paths.filter(path =>
      path.toLowerCase().includes(this.pathValue().toLowerCase())
    )
  );

  // Form groups
  projectInformationFormGroup: FormGroup;
  projectOriginFormGroup: FormGroup;
  updateMethodFormGroup: FormGroup;

  // Signals for validation state to avoid ExpressionChangedAfterItHasBeenCheckedError
  isStep1Valid = signal(false);
  isStep2Valid = signal(false);
  isStep3Valid = signal(false);

  constructor() {
    // Initialize forms
    this.projectInformationFormGroup = this.fb.group({
      projectName: ['', [Validators.required, Validators.maxLength(20)]],
      description: ['']
    });

    this.projectOriginFormGroup = this.fb.group({
      originType: [ContainerOriginType.Compose, Validators.required],
      gitUrl: [''],
      composeContent: ['', Validators.required],
      path: ['']
    });

    this.updateMethodFormGroup = this.fb.group({
      updateMethod: [ContainerUpdateMethod.Webhook, Validators.required],
      webhookUrl: [''],
      periodicInterval: ['']
    });
  }

  ngOnInit() {
    // Setup path autocomplete
    this.pathValue.set(this.projectOriginFormGroup.get('path')?.value || '');
    this.projectOriginFormGroup.get('path')?.valueChanges.subscribe(val => {
      this.pathValue.set(val || '');
    });

    // Initial validation setup
    this.updateValidationForOriginType(ContainerOriginType.Compose);
    // Initialize validation for preselected webhook
    this.updateValidationForUpdateMethod(ContainerUpdateMethod.Webhook);
    this.updateValidationStates();

    // Listen for form validation changes
    this.projectInformationFormGroup.statusChanges.subscribe(() => {
      this.updateValidationStates();
    });

    this.projectOriginFormGroup.statusChanges.subscribe(() => {
      this.updateValidationStates();
    });

    this.updateMethodFormGroup.statusChanges.subscribe(() => {
      this.updateValidationStates();
    });

    // Listen for origin type changes
    this.projectOriginFormGroup.get('originType')?.valueChanges.subscribe(originType => {
      this.updateValidationForOriginType(originType);
    });

    // Listen for update method changes
    this.updateMethodFormGroup.get('updateMethod')?.valueChanges.subscribe(updateMethod => {
      this.updateValidationForUpdateMethod(updateMethod);
    });
  }

  private updateValidationForOriginType(originType: ContainerOriginType) {
    const originControls = this.projectOriginFormGroup.controls;

    // Clear all validators first
    originControls['gitUrl'].clearValidators();
    originControls['composeContent'].clearValidators();
    originControls['path'].clearValidators();

    // Reset values for unused fields (but don't call updateValueAndValidity yet)
    originControls['gitUrl'].setValue('', { emitEvent: false });
    originControls['composeContent'].setValue('', { emitEvent: false });
    originControls['path'].setValue('', { emitEvent: false });

    // Reset update method form when origin type changes, but keep webhook preselected
    this.updateMethodFormGroup.get('updateMethod')?.setValue(ContainerUpdateMethod.Webhook, { emitEvent: false });
    this.updateMethodFormGroup.get('webhookUrl')?.setValue('', { emitEvent: false });
    this.updateMethodFormGroup.get('periodicInterval')?.setValue('', { emitEvent: false });

    // Apply validators based on origin type
    switch (originType) {
      case ContainerOriginType.Local:
        originControls['path'].setValidators([Validators.required]);
        break;

      case ContainerOriginType.Compose:
        originControls['composeContent'].setValidators([Validators.required]);
        break;

      case ContainerOriginType.Git:
        originControls['gitUrl'].setValidators([Validators.required]);
        break;
    }

    // Always require update method selection
    this.updateMethodFormGroup.get('updateMethod')?.setValidators([Validators.required]);

    // Update validity for relevant controls only
    originControls['gitUrl'].updateValueAndValidity();
    originControls['composeContent'].updateValueAndValidity();
    originControls['path'].updateValueAndValidity();
    this.updateMethodFormGroup.get('updateMethod')?.updateValueAndValidity();

    // Initialize validation for the preselected webhook
    this.updateValidationForUpdateMethod(ContainerUpdateMethod.Webhook);

    // Update validation states
    setTimeout(() => this.updateValidationStates(), 0);
  }

  private updateValidationForUpdateMethod(updateMethod: ContainerUpdateMethod) {
    const webhookControl = this.updateMethodFormGroup.get('webhookUrl');
    const intervalControl = this.updateMethodFormGroup.get('periodicInterval');

    // Clear validators
    webhookControl?.clearValidators();
    intervalControl?.clearValidators();

    // Reset values
    webhookControl?.setValue('', { emitEvent: false });
    intervalControl?.setValue('', { emitEvent: false });

    // Apply validators based on update method
    switch (updateMethod) {
      case ContainerUpdateMethod.Webhook:
        webhookControl?.setValidators([Validators.required, Validators.pattern('https?://.+')]);
        break;

      case ContainerUpdateMethod.Periodically:
        intervalControl?.setValidators([Validators.required, Validators.min(1)]);
        break;

      case ContainerUpdateMethod.Manual:
        // No additional validation needed
        break;
    }

    // Update validity
    webhookControl?.updateValueAndValidity();
    intervalControl?.updateValueAndValidity();

    // Update validation states
    setTimeout(() => this.updateValidationStates(), 0);
  }

  private updateValidationStates() {
    // Step 1 validation
    this.isStep1Valid.set(this.projectInformationFormGroup.valid);

    // Step 2 validation
    const originType = this.projectOriginFormGroup.get('originType')?.value;
    let step2Valid = false;

    switch (originType) {
      case ContainerOriginType.Local:
        step2Valid = !!this.projectOriginFormGroup.get('path')?.valid;
        break;
      case ContainerOriginType.Compose:
        step2Valid = !!this.projectOriginFormGroup.get('composeContent')?.valid;
        break;
      case ContainerOriginType.Git:
        step2Valid = !!this.projectOriginFormGroup.get('gitUrl')?.valid;
        break;
      default:
        step2Valid = false;
    }
    this.isStep2Valid.set(step2Valid);

    // Step 3 validation - always validate update method
    const updateMethod = this.updateMethodFormGroup.get('updateMethod')?.value;
    let step3Valid = false;

    if (!updateMethod) {
      step3Valid = false;
    } else {
      switch (updateMethod) {
        case ContainerUpdateMethod.Webhook:
          step3Valid = !!this.updateMethodFormGroup.get('webhookUrl')?.valid;
          break;
        case ContainerUpdateMethod.Periodically:
          step3Valid = !!this.updateMethodFormGroup.get('periodicInterval')?.valid;
          break;
        case ContainerUpdateMethod.Manual:
          step3Valid = true;
          break;
        default:
          step3Valid = false;
      }
    }
    this.isStep3Valid.set(step3Valid);
  }

  cancel() {
    this.dialogRef.close();
  }

  finish() {
    if (!this.isStep1Valid() || !this.isStep2Valid() || !this.isStep3Valid()) {
      return;
    }

    const result: CreateContainerRequest = {
      ...this.projectInformationFormGroup.value,
      ...this.projectOriginFormGroup.value,
      ...this.updateMethodFormGroup.value
    };

    this.dialogRef.close(result);
  }
}
