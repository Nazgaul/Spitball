import {mapActions} from 'vuex'
import nav from '../../../services/navigation/vertical-navigation/nav'

export default {

    data() {
        return {
            bannerData: {
                title: "",
                text: "",
                lineColor :"#ffffff"
            },
        }
    },

    watch: {
        '$route': 'getBannerData',

    },
    methods: {
        ...mapActions(['hideRegistrationBanner']),


        getBannerData() {
            console.log(nav[this.$route.path.slice(1)].banner)
            return  this.bannerData = nav[this.$route.path.slice(1)].banner;
        }
    },
    created(){
        this.getBannerData()
    }

}