<template>
    <div class="tutorRequest-success">
        <div class="tutorRequest-success-middle">
            <img class="success-img" src="../images/success.png" alt="">
            <p v-language:inner="'tutorRequest_success'"/>
        </div>
        <div class="tutorRequest-success-bottom">
            <v-btn sel="cancel_tutor_request" @click="tutorRequestDialogClose" 
                class="tutorRequest-btn-back" color="white" depressed round>
                <span v-language:inner="'tutorRequest_btn_close'"/>
            </v-btn>
        </div>
    </div>
</template>

<script>
import { mapActions } from 'vuex';

export default {
    name:'tutorRequestSuccess',
    methods: {
        ...mapActions(['updateRequestDialog','resetRequestTutor']),
        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
            this.resetRequestTutor()
        },
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.tutorRequest-success{
        width: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: space-between;
        height: 100%;
        min-height: 100%;
        text-align: center;
        .tutorRequest-success-middle{
            padding: 100px 0 120px 0;
            height: 100%;
            display: flex;
            flex-direction: column;
            align-items: center; 
            justify-content: center;
            .success-img{
                width: 140px;
                height: 88px;
                object-fit: contain;

            }
            p{
                margin: 0;
                margin-top: 20px;
                padding: 0;
                font-size: 16px;
                font-weight: 600;
                line-height: 1.5;
                color: @global-purple;
                white-space: pre-line;
            }
        }
    .tutorRequest-success-bottom{
        .v-btn{
            @media (max-width: @screen-xs) {
              min-width: 120px;  
            }
            min-width: 140px;
            height: 40px !important;
            padding: 0px 32px !important;
            text-transform: capitalize  !important;
        }
        .tutorRequest-btn-back{
            font-weight: 600;
            color: @global-blue;
            border: 1px solid @global-blue !important;
        }
    }
}
</style>