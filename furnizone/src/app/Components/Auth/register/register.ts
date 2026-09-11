import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Auth } from '../../../Core/Services/auth';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-register',
  imports: [
     ReactiveFormsModule,
CommonModule,
RouterModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private auth=inject(Auth);
  private subs=new Subscription();

  showPassword = signal(false);
  showConfirmPassword = signal(false);
  isLoading = signal(false);
  registerError = signal('');


  private touchedFields = signal<{ [key: string]: boolean }>({});


  registerForm: FormGroup = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/(?=.*[A-Z])(?=.*[0-9])/)
    ]],
    confirmPassword: ['', [Validators.required]],
    agreeTerms: [false, [Validators.requiredTrue]]
  }, { validators: this.passwordMatchValidator });


  get email() { return this.registerForm.get('email')!; }
  get password() { return this.registerForm.get('password')!; }
  get agreeTerms() { return this.registerForm.get('agreeTerms')!; }

  // 5. ميثود لتسجيل أن الحقل تم لمسه (عند الـ Blur في الـ HTML)
  markTouched(fieldName: string): void {
    this.touchedFields.update(prev => ({ ...prev, [fieldName]: true }));
    // برضه بنسمع في الـ Form Control الأصلي بتاع أنجلر
    this.registerForm.get(fieldName)?.markAsTouched();
  }

  
  isTouched(fieldName: string): boolean {
    return !!this.touchedFields()[fieldName] || !!this.registerForm.get(fieldName)?.touched;
  }

  getFieldError(fieldName: string) {
    return computed(() => {
    
      this.touchedFields(); 
      const control = this.registerForm.get(fieldName);
      
      if (!control || (!control.touched && !this.isTouched(fieldName))) return '';

      if (control.hasError('required')) return 'This field is required';
      if (control.hasError('email')) return 'Please enter a valid email address';
      if (control.hasError('minlength')) {
        const requiredLength = control.errors?.['minlength']?.requiredLength;
        return `Must be at least ${requiredLength} characters`;
      }
      if (control.hasError('pattern') && fieldName === 'password') {
        return 'Password must contain at least 1 uppercase letter and 1 number';
      }
      
      if (control.hasError('passwordMismatch') && fieldName === 'confirmPassword') {
        return 'Passwords do not match';
      }

      return '';
    });
  }

  
  passwordStrength = computed(() => {
    const pass = this.registerForm.value.password || '';
    if (!pass) return 0;

    let strength = 0;
    if (pass.length >= 8) strength++; 
    if (/[A-Z]/.test(pass)) strength++; 
    if (/[0-9]/.test(pass)) strength++; 
    if (/[^A-Za-z0-9]/.test(pass)) strength++; 

    return strength === 0 ? 1 : strength; 
  });


  passwordStrengthColor = computed(() => {
    const strength = this.passwordStrength();
    if (strength === 1) return 'bg-red-500';
    if (strength === 2) return 'bg-yellow-500';
    if (strength === 3) return 'bg-blue-500';
    return 'bg-green-600'; 
  });


  passwordStrengthLabel = computed(() => {
    const strength = this.passwordStrength();
    if (strength === 1) return 'Weak';
    if (strength === 2) return 'Fair';
    if (strength === 3) return 'Good';
    return 'Strong';
  });

  
  togglePassword(): void {
    this.showPassword.update(v => !v);
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword.update(v => !v);
  }

  // 9. Custom Validator لمطابقة كلمة المرور مع التأكيد
  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    // لو مفيش مشكلة بنشيل خطأ الماتش بس بنحافظ على الأخطاء التانية لو موجودة
    if (confirmPassword?.hasError('passwordMismatch')) {
      confirmPassword.setErrors(null);
    }
    return null;
  }

  private rout=inject(Router);
  
  onSubmit(): void {
    this.registerError.set('');
    Object.keys(this.registerForm.controls).forEach(key => {
      this.markTouched(key);
    });
    if (this.registerForm.invalid) return;
    this.isLoading.set(true);
    this.subs.add(
      this.auth.signUp(this.registerForm.value)
      .subscribe({
        next:(res:any)=>{
          console.log(res);
          const token=res.data.token;
          localStorage.setItem('token',token);
          if(res.data.role==0){
          localStorage.setItem('role','user');
          this.auth.role.set('user');
          }
          this.auth.isLogged.set(true);
          alert('تم تسجيل حسابك بنجاح')
          this.rout.navigate(['/home'])
        }
      })
    )
  }


}
