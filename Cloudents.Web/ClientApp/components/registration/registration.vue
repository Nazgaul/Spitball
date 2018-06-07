<template>
    <div class="registration">
        <button class="back-button" @click="showDialog = true">
            <v-icon right>sbf-close</v-icon>
        </button>
        <div class="form-wrap">
            <register-email v-if="step===0" @next="nextStep"></register-email>
            <register-phone v-else-if="step===1" @next="nextStep"></register-phone>
            <register-account v-else-if="step===2"></register-account>
        </div>
        <div class="img-wrap"></div>
        <div class="progress">
            <div v-for="page in [0,1,2]" :class="{highlighted: page===step}"></div>
        </div>

        <v-dialog v-model="showDialog" max-width="600px">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to exit?</h1>
                    <p>Exit from this process will delete all your progress and information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>

<style src="./registration.less" lang="less"></style>
<script src="./registration.js"></script>
