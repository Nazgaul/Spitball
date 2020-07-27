import { connectivityModule } from "../connectivity.module";

const BASE_URL = '/wallet';

export default {
   get: {
      balance: () => connectivityModule.http.get(`${BASE_URL}/balance`),
      paymentLink: () => connectivityModule.http.get(`${BASE_URL}/getPaymentLink`),
   },
   post: {
      redeem: (amount) => connectivityModule.http.post(`${BASE_URL}/redeem`, {amount}),
   },
}