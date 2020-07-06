import {dashboardRoutes} from './routes/dashboardRoutes.js';
import {profileRoutes} from './routes/profileRoutes.js';
import {studyRoomRoutes} from './routes/studyRoomRoutes.js';
import {registrationRoutes} from './routes/registrationRoutes.js';
import {landingRoutes} from './routes/landingRoutes.js';
import {questionRoutes} from './routes/questionRoutes.js';
import {itemRoutes} from './routes/itemRoutes.js';
import {feedRoutes} from './routes/feedRoutes.js';
import {globalRoutes} from './routes/globalRoutes.js';
import {marketingRoutes} from './routes/marketingRoutes.js';
import {messageCenterRoutes} from './routes/messageCenterRoutes.js';
import {courseRoutes} from './routes/courseRoutes.js'
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
    ...globalRoutes,
    ...messageCenterRoutes,
    ...courseRoutes
];
export const routes = routes2;
