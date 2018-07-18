<template>
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <slot>
        <div class="form-wrap">
            <component :is="`register-email`" @next="nextStep"></component>
        </div>

        <div class="img-wrap"></div>
        <div class="progress">
            <div v-for="page in REGISTRATION_STEPS" :class="{highlighted: page===step}"></div>
        </div>
        </slot>
        <button class="back-button" @click="showDialog = true">
            <v-icon right>sbf-close</v-icon>
        </button>
        <v-dialog v-model="showDialog" max-width="600px" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to exit?</h1>
                    <p>Exiting from this process will delete all your progress and information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>

<style src="./registration.less" lang="less"></style>
<script src="./registration.js"></script>
