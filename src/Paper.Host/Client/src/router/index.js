import Vue from 'vue'
import Router from 'vue-router'
import NotFound from '@/components/NotFoundPage'
import ErrorPage from '@/components/ErrorPage'
import AboutPage from '@/components/AboutPage'
import FormPage from '@/components/FormPage'
import LoginPage from '@/components/LoginPage'
import Page from '@/components/PageView'
import Home from '@/components/Home'
// import store from '../store/store'

Vue.use(Router)

// const ifNotAuthenticated = (to, from, next) => {
//   if (!store.getters['auth/isAuthenticated']) {
//     next()
//     return
//   }
//   next('/')
// }

// const ifAuthenticated = (to, from, next) => {
//   if (store.getters['auth/isAuthenticated']) {
//     next()
//     return
//   }
//   next('/login')
// }

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/page/:path(.*)*',
      name: 'page',
      component: Page
      // ,
      // beforeEnter: ifAuthenticated
    },
    {
      path: '/notFound',
      name: 'notFound',
      component: NotFound,
      props: true
    },
    {
      path: '/error',
      name: 'error',
      component: ErrorPage,
      props: true
    },
    {
      path: '/about',
      name: 'about',
      component: AboutPage
      // ,
      // beforeEnter: ifAuthenticated
    },
    {
      path: '/form/:path(.*)*',
      name: 'form',
      component: FormPage
    },
    {
      path: '/login',
      name: 'login',
      component: LoginPage
      // ,
      // beforeEnter: ifNotAuthenticated
    }
  ]
})
