<template functional>
    <v-container class="features" grid-list-xs>
        <v-layout row justify-space-around wrap>
            <v-flex v-for="(feature, index) in props.features" :key="index" class="feature" md4 xs12 pa-3>
                <v-layout class="wrapper" row align-start justify-space-between d-flex wrap>
                    <v-flex class="icon-wrapper" md12>
                        <slot :type="`feature-${index}`" name="item" ></slot>
                    </v-flex>
                    <v-flex md12>
                        <div class="data text-xs-left text-sm-center">
                            <h3>{{feature.title}}</h3>
                            <p class="hidden-xs-only">{{feature.text}}</p>
                            <p class="hidden-sm-and-up">{{feature.mobileText}}</p>
                        </div>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>


<style src="./features.less" lang="less" scoped></style>
