export interface User {
  id: string;
  displayName: string;
  email: string;
  token: string;
  imageUrl?: string;
  roles: string[];
}

export interface LoginCreds {
  email: string;
  password: string;
}

export interface RegisterCreds {
  displayName: string;
  email: string;
  password: string;
  gender: string;
  dateOfBirth: string;
  city: string;
  country: string;
}
