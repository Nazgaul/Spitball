import { connectivityModule } from "../connectivity.module";

const BASE_URL = '/profile';

export default {
   get: {
      profile: (id) => connectivityModule.http.get(`${BASE_URL}/${id}`),
      reviews: (id) => connectivityModule.http.get(`${BASE_URL}/${id}/about`),
      documents: (id, params) => connectivityModule.http.get(`${BASE_URL}/${id}/documents`, { params }),
   },
   post: {
      follow: (id) => connectivityModule.http.post(`${BASE_URL}/follow`, { id }),
   },
   delete: {
      unfollow: (id) => connectivityModule.http.delete(`${BASE_URL}/unfollow/${id}`),
   }
}