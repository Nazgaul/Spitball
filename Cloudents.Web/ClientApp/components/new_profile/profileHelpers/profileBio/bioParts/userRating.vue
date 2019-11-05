<template>
        <v-layout align-center justify-center class="rating-container">
            <v-flex shrink :class="{'mr-2': $vuetify.breakpoint.smAndUp}">
                <v-rating :class="['ratingCmp',{'rtl-rating': isRtl}]"
                        v-model="dynamicRating"
                        :color="starColor"
                        :background-color="starColor"
                        :length="ratingLength"
                        half-icon="sbf-star-rating-half"
                        full-icon="sbf-star-rating-full"
                        empty-icon="sbf-star-rating-empty"
                        :readonly="readonly"
                        :size="size"
                        :hover="true"
                ></v-rating>
                <!--  :hover="!isRtl" hover binded to rtl, if rtl true prevent hover effect cause of half star wrong side animation-->
            </v-flex>
            <v-flex>
                <div class="">
                   <span v-show="showRateNumber" :style="{ color: rateNumColor }" class="caption ml-1 pb-1 rating-number">
       {{ dynamicRating }}
      </span>
                </div>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "userReviews",
        data() {
            return {
                ratingLength: 5,
                isRtl: global.isRtl
            }
        },
        props: {
            callbackFn: {
                type: Function,
                required: false
            },
            starColor: {
                type: String,
                default: '#ffca54'
            },
            readonly:{
                type: Boolean,
                default: true
            },
            size:{
                type: String,
                default: '24'
            },
            rateNumColor:{
                type: String,
                default: '#ffffff'
            },
            rating: {
                type: Number,
                default: 0
            },
            showRateNumber:{
                type: Boolean,
                default: true
            }
        },
        computed: {
            dynamicRating:{
             get(){
                 if(this.rating > 0){
                    return Number.parseFloat(Number.parseFloat(this.rating).toFixed(2));
                 }else{
                    return this.rating
                 }
             },
            set(newValue){
                if(this.callbackFn){
                    this.callbackFn(newValue)
                }
            }
            }
        },
    }
</script>

<style lang="less">
    .rating-container{
        .ratingCmp{
            display: flex;
        }
        .rtl-rating{
            .sbf-star-rating-half{
                -moz-transform: scaleX(-1)/*rtl:ignore*/;
                -o-transform: scaleX(-1)/*rtl:ignore*/;
                -webkit-transform: scaleX(-1)/*rtl:ignore*/;
                transform: scaleX(-1)/*rtl:ignore*/;

            }
        }
        .v-rating{
            .v-icon{
                padding: 0;
            }
        }
    }

</style>