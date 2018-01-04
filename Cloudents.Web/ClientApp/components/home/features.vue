<template functional>
    <v-container class="features limited-width" pa-0>
        <v-divider class="hidden-xs-only"></v-divider>
        <v-layout row justify-space-around>
            <v-flex v-for="(feature, index) in props.features" :key="index" :class="`feature ${index}`" xs4 px-1 mb-3 text-xs-center>
                <v-layout class="wrapper" column align-top justify-space-between d-flex wrap>
                    <v-flex class="icon-wrapper" row d-flex align-center justify-center xs12>
                        <slot :type="feature.icon" name="item"></slot>
                    </v-flex>
                    <v-flex xs12>
                        <div class="data text-xs-center">
                            <h3>{{feature.title}}</h3>
                            <p class="hidden-sm-and-down">{{feature.text}}</p>
                        </div>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>


<style src="./features.less" lang="less"></style>
