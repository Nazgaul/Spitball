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
        <span class="right ml-2 ml-sm-0" style="cursor:pointer">
            <likeFilledSVG v-if="isLiked" @click.stop.prevent="upvoteDocument" width="18" class="likeSVG" />
            <likeSVG v-if="!isLiked" @click.stop.prevent="upvoteDocument" width="18" class="likeSVG" />
            <span v-if="item.votes>0">{{item.votes}}</span>
        </span>
        <v-spacer v-if="isMobile"></v-spacer>
        <documentPrice :price="item.price" v-if="isMobile" />
    </v-flex>
</template>

<script>
const documentPrice = () => import('../../pages/global/documentPrice/documentPrice.vue')

const likeSVG = () => import("../img/like.svg");
const likeFilledSVG = () => import("../img/like-filled.svg");

export default {
    components: {
        documentPrice,
        likeSVG,
        likeFilledSVG,
    },
    props: {
        item: {
            required: true
        }
    },
    data() {
        return {
            isLiked: false,
        }
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        docViews() {
            return this.item.views;
        },
        docDownloads() {
            return this.item.downloads;
        },
        docPurchased() {
            return this.item.purchased;
        },
        logged() {
            return this.$store.getters.getUserLoggedInStatus;
        }
    },
    methods: {
        upvoteDocument(e) {
            e.stopImmediatePropagation();
            if (!this.logged)  {
                this.$store.commit('setComponent', 'register')
                return
            }
            this.isLiked = true;
            let type = "up";
            if (this.item.upvoted) {
                type = "none";
                this.isLiked = false;
            }
            let data = { type, id: this.item.id };
            this.$store.dispatch('documentVote', data);
        }
    },
    created() {
        this.isLiked = this.item.upvoted;
    }
}
</script>