<template xmlns="http://www.w3.org/1999/html">
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>

        <component :is="`step_${stepNumber}`"
                   :passScoreObj="passScoreObj"
                   :isMobile="isMobile"
                   :meta="meta"
                   :isCampaignOn="isCampaignOn"
                   :userEmail="userEmail"
                   :phone="phone"
                   :lastActiveRoute="lastActiveRoute"
                   :toUrl="toUrl"
                   :passResetCode="passResetCode"
                   :ID="ID"
                   :camefromCreate="camefromCreate"
                   :campaignData="campaignData"
                   :isSignIn="isSignInComputed"
                   :isNewUser="isNewUser"></component>
        <button class="back-button" @click="showDialog = true" v-if="stepNumber !== 6">
            <v-icon right>sbf-close</v-icon>
        </button>

        <!--exit dialog-->
        <v-dialog v-model="showDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1 v-if="isMobile">
                        <span v-html="resource.mobile"></span>
                    </h1>
                    <h1 v-else v-language:inner>login_are_you_sure_you_want_to_exit</h1>
                    <p><span class="pre-line" v-language:inner>login_exiting_information1</span><br /></p>

                    <v-btn v-if="isMobile" class="continue-registr"
                           @click="showDialog = false">
                        <span v-language:inner>login_continue_registration</span>
                    </v-btn>
                    <button class="continue-btn" @click="$_back" v-language:inner>login_Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>
    </div>
</template>
<script src="./login.js"></script>
<style lang="less" src="./login.less">
</style>