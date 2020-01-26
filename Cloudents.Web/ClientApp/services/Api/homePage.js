import { connectivityModule } from "../connectivity.module";

const BASE_URL = '/homePage';

export default {
   get: {
      banner: () => connectivityModule.http.get(`${BASE_URL}/banner`),
      stats: () => connectivityModule.http.get(`${BASE_URL}`),
      subjects: (params) => connectivityModule.http.get(`${BASE_URL}/subjects`,params),
      reviews: (params) => connectivityModule.http.get(`${BASE_URL}/reviews`,params),
      items: (params) => connectivityModule.http.get(`${BASE_URL}/documents`,params),
      tutors: (params) => connectivityModule.http.get(`${BASE_URL}/tutors`,params),
   },
}