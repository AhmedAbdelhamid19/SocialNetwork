export type Member = {
  id: number
  dateOfBirth: string
  imageUrl?: string
  displayName: string
  created: string
  lastActive: string
  gender: string
  description?: string
  city: string
  country: string
}

export type Photo = {
  id: number
  url: string
  publicId?: string
  memberId: number
}

export type EditMember = {
  displayName: string
  description?: string
  city: string
  country: string
}

export class MemberParams {
  gender?: string;
  minAge  = 18;
  maxAge = 100;
  pageSize = 5;
  pageNumber = 1;
}