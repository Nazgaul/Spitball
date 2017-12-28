<template functional>
    <v-container class="features" grid-list-xs>
        <v-layout row justify-space-around wrap>
            <v-flex v-for="(feature, index) in props.features" :key="index" :class="`feature ${index}`" xs4 px-3 mb-3>
                <v-layout class="wrapper" column align-top justify-space-between d-flex wrap>
                    <v-flex class="icon-wrapper" xs12>
                        <slot :type="feature.icon" name="item" ></slot>
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
