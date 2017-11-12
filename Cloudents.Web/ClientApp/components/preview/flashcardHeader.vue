<template functional>
    <v-layout row justify-space-between>
        <v-flex xs6>
            <div v-if="props.showCards">{{props.name}} </div>
            <v-progress-linear v-if="props.showCards" height="10" color="info" :value="((props.currentIndex+1)/props.cardsSize)*100"></v-progress-linear>
            <v-layout v-if="props.showCards" row justify-space-between> <v-flex xs2>progress</v-flex><v-flex xs2 class="text-xs-right"><span>{{(props.currentIndex+1)}}/{{props.cardsSize}}</span></v-flex></v-layout>
        </v-flex><v-flex xs1><close-action></close-action></v-flex>
    </v-layout>
</template>