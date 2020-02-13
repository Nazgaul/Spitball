import Api from './Api/homePage.js';
import { Banner } from './Dto/banner.js';
import { HomePage } from './Dto/homePage.js';
import { Item } from './Dto/item.js';
import { User } from './Dto/user.js';

export default {
    async getHomePageTutors(count = 12) {
        let params = {count}
        let { data } = await Api.get.tutors({params})
        return data.map(tutor=>new User.TutorItem(tutor));
    },
    async getHomePageItems(count = 12) {
        let params = {count}
        let { data } = await Api.get.items({params})
        return data.map(item=>new Item[item.documentType](item));
    },
    async getHomePageReviews(count = 3) {
        let params = {count}
        let { data } = await Api.get.reviews({params})
        return data.map(r=>new HomePage.Review(r));
    },
    async getHomePageSubjects(count = 12) {
        // TODO clean it !!! ask ram
        let params = {count}
        let { data } = await Api.get.subjects(params)
        return data.map(subject=>subject)
    },
    async getHomePageStats() {
        let { data } = await Api.get.stats()
        return new HomePage.Stats(data);
    },
    async getBannerParams() {
      let { data } = await Api.get.banner()
      return data === null? null : new Banner.Default(data);
    }
}