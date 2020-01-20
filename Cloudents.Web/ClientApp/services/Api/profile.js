import { connectivityModule } from "../connectivity.module";

const BASE_URL = '/Profile';

export default {
   get: {
      profile: (id) => {
         return connectivityModule.http.get(`${BASE_URL}/${id}`);
      },
      reviews: (id) => {
         return connectivityModule.http.get(`${BASE_URL}/${id}/about`);
      },
      documents: (id, page, pageSize) => {
         let strPage = `?page=${page}&pageSize=${pageSize}`;
         return connectivityModule.http.get(`${BASE_URL}/${id}/documents/${strPage}`);
      },
   },
   post: {
      follow: (id) => {
         return connectivityModule.http.post(`${BASE_URL}/follow`, { id });
      }
   },
   delete: {
      unfollow: (id) => {
         return connectivityModule.http.delete(`${BASE_URL}/unFollow/${id}`);
      }
   }
}