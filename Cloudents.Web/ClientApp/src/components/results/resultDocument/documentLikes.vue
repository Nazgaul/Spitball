<template>
    <v-flex grow class="bottom-row">
        <div class="left">
            <!-- <span v-if="docViews" class="views-cont">
                <span>{{ $tc('resultNote_view',docViews)}}</span>
            </span>
            <span v-if="docDownloads && !item.price">
                <span>{{ $tc('resultNote_download',docDownloads)}}</span>
            </span>
            <span v-if="docPurchased && item.price">
                <span>{{ $tc('resultNote_purchased',docPurchased)}}</span>
            </span> -->
        </div>
        <v-spacer v-if="isMobile"></v-spacer>
        <documentPrice :price="item.price" v-if="isMobile" :isSubscribed="isSubscribed" />
    </v-flex>
</template>

<script>
const documentPrice = () => import('../../pages/global/documentPrice/documentPrice.vue')

export default {
    components: {
        documentPrice,
    },
    props: {
        item: {
            required: true
        }
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        isSubscribed() {
            return this.item.priceType === 'Subscriber'
        }
        // docViews() {
        //     return this.item.views;
        // },
        // docDownloads() {
        //     return this.item.downloads;
        // },
        // docPurchased() {
        //     return this.item.purchased;
        // },
        // logged() {
        //     return this.$store.getters.getUserLoggedInStatus;
        // }
    }
}
</script>