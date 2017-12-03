<template>
      <v-dialog v-model="showDialog" max-width="518">
        <v-card>
            <v-card-title class="">Personalize Results</v-card-title>
            <v-card-text>You can tailor the results to you by adding your school and classes.</v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn flat="flat" @click.native="showDialog = false">Not now</v-btn>
                <v-btn flat="flat" @click.native="showDialog = false">Personalize</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
    
</template>
<style lang="less" scoped>
    @import "../../mixin.less";
    .card{
        border-radius:6px;
    } 
    .card__title {
        font-size: 36px;
        font-weight: 300;
        letter-spacing: -0.5px;
        text-align: center;
        color: @Spitball-Text;
        display: block; //override
    }
    .card__text {
        font-size: 16px;
        letter-spacing: -0.3px;
        text-align: center;
        max-width: 360px;
        margin: 0 auto;
        line-height: 1.38;
        color: @Feed-Like-count;
    }
    .btn {
        text-transform: capitalize;
        font-size: 16px;
        letter-spacing: -0.3px;
        color: @Spitball-Main-color;
    }
</style>
<script>
    import searchItem from './searchItem.vue'
    import { searchObjects } from './consts'
    import { mapGetters } from 'vuex'
    export default {
        data() {
            return { showDialog: true }
        },


        computed: {
            ...mapGetters(['getUniversity']),
            searchType: function () { return this.getUniversity ? 'course' : 'university' }
        },

        props: { show: {} },
        watch: {
            show: function (val) {
                if (val) {
                this.showDialog = true;
                }
            }
        }
    }
</script>