
<template>
    <div>
        <marketingActions :resource="resource"></marketingActions>
        <marketingBlogs></marketingBlogs>
        <tableCoupon></tableCoupon>
    </div>
</template>

<script>
import intercomService from "../../../../services/intercomService";
import * as routeName from '../../../../routes/routeNames';

const marketingActions = () => import('../marketingActions/marketingActions.vue');
const marketingBlogs = () => import('../marketingBlogs/marketingBlogs.vue');
const tableCoupon = () => import('../tableCoupon/tableCoupon.vue');

export default {
    name: 'marketing',
    components: {
        marketingActions,
        marketingBlogs,
        tableCoupon,
    },
    data() {
        return {
            resource: {
                box1: {
                    title1: this.$t('marketing_sharePost_title1'),
                    title2: this.$t('marketing_sharePost_title2'),
                    image: require('../images/sharePost.png'),
                    buttonText: this.$t('marketing_lets_go'),
                    action: this.action
                },
                box2: {
                    title1: this.$t('marketing_createOffer_title1'),
                    title2: this.$t('marketing_createOffer_title2'),
                    image: require('../images/specialOffer.png'),
                    buttonText: this.$t('marketing_get_started'),
                    action: this.openDialog
                },
                box3: {
                    title1: this.$t('marketing_createVideo_title1'),
                    title2: this.$t('marketing_createVideo_title2'),
                    image: require('../images/createVideo.png'),
                    buttonText: this.$t('marketing_get_started'),
                    action: this.openIntercom
                }
            }
        }
    },
    methods: {
        action() {
            this.$router.push({name: routeName.MarketingPromote})
        },
        openDialog() {
            this.$openDialog('createCoupon')
        },
        openIntercom() {
            intercomService.showDialog();
        }
    }
}
</script>