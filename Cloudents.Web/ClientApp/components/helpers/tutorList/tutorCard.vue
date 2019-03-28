<template>
    <v-card class="tutor-card-wrap px-3">
    <v-layout>
        <v-flex xs6 sm6 md6>
            <span class="tutor-name font-weight-bold caption">{{tutorData.name}}</span>
        </v-flex>
        <v-flex>
            <user-rank :score="tutorData.score"></user-rank>
        </v-flex>
        <v-flex>
           <span class="d-flex rounded align-center justify-center">
                <commentIcon class="chat-icon white"></commentIcon>
           </span>
        </v-flex>
    </v-layout>
    <v-layout>
        <v-flex>
                <!--<img src="" alt="">-->
            <span class="img-placeholder"></span>
        </v-flex>
        <v-flex>
            <span class="tutor-text caption">
              {{tutorData.text}}
            </span>
        </v-flex>
    </v-layout>
    <v-layout>
        <v-flex>
            <userRating
                        :rating="3.5"
                        :starColor="'#43425D'"
                        :size="'18'"
                        :rate-num-color="'#43425D'"></userRating>
        </v-flex>
        <v-flex>
            <span class="pricing">
                <span class="font-weight-medium subheading pricing">{{tutorData.price}}</span>
                <span class="small-text pricing" v-language:inner>app_currency_dynamic</span>
                 <span class="pricing small-text">/hour</span>
            </span>
        </v-flex>
    </v-layout>
    </v-card>
</template>

<script>
    import userRank from '../UserRank/UserRank.vue';
    import userRating from '../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import commentIcon from '../../../font-icon/comment-icon.svg';
    export default {
        name: "tutorCard",
        components: {
            userRank,
            userRating,
            commentIcon
        },
        data() {
            return {
            };
        },
        props: {
            tutorData: {
            },
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.tutor-card-wrap{
    //delete when connected with real data
    .img-placeholder{
        height: 40px;
        width: 40px;
        border-radius: 50%;
        background-color: @profileTextColor;
    }
    .pricing{
     font-family: @fontOpenSans;
     color:@purpleLight;
    }
    .small-text{
        font-size: 10px;
    }
    .tutor-text{
        font-family: @fontOpenSans;
        font-size: 12px;
        color: @profileTextColor;
    }
    .rounded{
        height: 32px;
        width: 32px;
        border-radius: 50%;
        background-color: @profileTextColor;
    }

}
</style>