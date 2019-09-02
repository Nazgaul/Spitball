<template>
    <div class="tutorRequest-bottom">
        <v-btn @click="tutorRequestDialogClose" class="tutorRequest-btn-back" color="white" depressed round>
            <span>Cancel</span>
        </v-btn>
        <v-btn class="tutorRequest-btn-next" depressed round color="#4452fc" >
            <span>Next</span>
        </v-btn>
    </div>
</template>

<script>
import { mapActions } from 'vuex';
export default {
    methods: {
        ...mapActions(['updateRequestDialog']),
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutorRequest-bottom{
        .v-btn{
            min-width: 140px;
            height: 40px !important;
            padding: 0px 32px !important;
        }
        .tutorRequest-btn-back{
            color: @global-blue;
            border: 1px solid @global-blue !important;
        }
        .tutorRequest-btn-next{
            min-width: 140px;
            color: white !important;
            text-transform: capitalize  !important;
            font-size: 16px;
            font-weight: 600;
            letter-spacing: -0.3px;
        }
    }
</style>