<template>
    <div class="marketingPromote">
        <marketingActions #closeIcon :resource="resource">
            <v-icon color="#adadba" size="12">sbf-close</v-icon>
        </marketingActions>
    </div>
</template>

<script>
import marketingActions from '../marketingActions/marketingActions.vue';

export default {
    components: {
        marketingActions,
    },
    data() {
        return {
            resource: {
                box1: {
                    title1: this.$t('promote_sharePost_title1'),
                    title2: this.$t('promote_sharePost_title2'),
                    image: require('../images/promoteProfile.png'),
                    buttonText: this.$t('promote_lets_go'),
                    action: this.action
                },
                box2: {
                    title1: this.$t('promote_createOffer_title1'),
                    title2: this.$t('promote_createOffer_title2'),
                    image: require('../images/promoteVideo.png'),
                    buttonText: this.$t('promote_lets_go'),
                    action: this.action
                },
                box3: {
                    title1: this.$t('promote_createVideo_title1'),
                    title2: this.$t('promote_createVideo_title2'),
                    image: require('../images/promoteContent.png'),
                    buttonText: this.$t('promote_lets_go'),
                    action: this.action
                }
            }
        }
    },

    methods: {
        action() {
            console.log("dasjfnaskjfbkfsea");
        }
    }
}
</script>