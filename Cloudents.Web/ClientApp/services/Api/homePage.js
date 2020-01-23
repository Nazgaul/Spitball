import { connectivityModule } from "../connectivity.module";

const BASE_URL = '/homePage';

export default {
   get: {
      banner: () => connectivityModule.http.get(`${BASE_URL}/banner`),
   },
}