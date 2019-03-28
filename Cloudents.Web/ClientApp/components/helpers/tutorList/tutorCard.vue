<template>
    <v-card class="tutor-card-wrap px-3 pb-2 mb-2 elevation-0">
    <v-layout align-center class="pt-3">
        <v-flex shrink class="mr-2 tutor-name font-weight-bold caption">
            {{tutorData.name}}
        </v-flex>
        <v-flex grow>
            <user-rank class="tutor-rank" :score="tutorData.score"></user-rank>
        </v-flex>
        <v-flex class="rounded" shrink>
            <commentIcon class="chat-icon"></commentIcon>
        </v-flex>
    </v-layout>
    <v-layout align-center justify-center class="pb-1">
        <v-flex>
                <img class="rounded-img" src="https://spitball-dev-function.azureedge.net/api/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMzM2LzE1NTM2OTk0NjMucG5nEAA??&width=40&height=40&mode=crop" alt="">
        </v-flex>
        <v-flex class="pl-3 tutor-text caption" >
            {{tutorData.text}}
        </v-flex>
    </v-layout>
        <v-divider></v-divider>
    <v-layout class="pt-1" align-center justify-center>
        <v-flex xs6 grow>
            <userRating
                        :rating="tutorData.rating"
                        :starColor="'#43425D'"
                        :rateNumColor="'#43425D'"
                        :size="'15'"
                        :rate-num-color="'#43425D'"></userRating>
        </v-flex>
        <v-flex xs8 class="text-xs-right">
            <span class="pricing">
                <span class="font-weight-medium subheading pricing">{{tutorData.price}}</span>
                <span class="small-text pricing" v-language:inner>app_currency_dynamic</span>
                 <span class="pricing small-text">/ <span v-language:inner>tutorList_per_hour</span></span>
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
    border-radius: 4px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
    .rounded-img{
        border-radius: 50%;
    }
    .tutor-name{
        color: @profileTextColor;
        line-height: 12px;
    }
    .tutor-rank{
            font-size: 10px;
            margin-top: 0;
            line-height: 14px;
            width: 60px;
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
        line-height: 1.33;
    }
    .rounded{
        height: 32px;
        width: 32px;
        border-radius: 50%;
        background-color: @profileTextColor;
    }
    .chat-icon{
        margin: 8px auto;
        display: block;
        fill: @color-white;
    }

}
</style>