import {dashboardRoutes} from './routes/dashboardRoutes.js';
import {profileRoutes} from './routes/profileRoutes.js';
import {studyRoomRoutes} from './routes/studyRoomRoutes.js';
import {registrationRoutes} from './routes/registrationRoutes.js';
import {landingRoutes} from './routes/landingRoutes.js';
import {questionRoutes} from './routes/questionRoutes.js';
import {itemRoutes} from './routes/itemRoutes.js';
import {feedRoutes} from './routes/feedRoutes.js';
import {courseRoutes} from './routes/courseRoutes.js';
import {universityRoutes} from './routes/universityRoutes.js';
import {globalRoutes} from './routes/globalRoutes.js';
import {marketingRoutes} from './routes/marketingRoutes.js';

let routes2 = [
    ...landingRoutes,
    ...registrationRoutes,
    ...studyRoomRoutes,
    ...marketingRoutes,
    ...profileRoutes,
    ...dashboardRoutes,
    ...questionRoutes,
    ...itemRoutes,
    ...feedRoutes,
    ...courseRoutes,
    ...universityRoutes,
    ...globalRoutes,
];
export const routes = routes2;
