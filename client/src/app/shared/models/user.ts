export type User = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  paswword: string;
  phoneNumber?: string;
  city?: string;
  roles: string | string[];
};
