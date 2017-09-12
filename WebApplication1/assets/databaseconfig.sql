CREATE DATABASE IF NOT EXISTS suntech;
use suntech;

CREATE TABLE IF NOT EXISTS `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  `enabled` int(1) NOT NULL DEFAULT '1',
  `registerdate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `name` varchar(100) NOT NULL,
  `surname` varchar(100) NOT NULL,
  `email` varchar(50) NOT NULL,
  `phone` varchar(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=997 DEFAULT CHARSET=utf8;

DELETE FROM `user`;
INSERT INTO `user` (`id`, `username`, `password`, `enabled`, `registerdate`, `name`, `surname`, `email`, `phone`) VALUES
	(1001, 'username0', 'senha0', 1, '2017-09-12 11:18:31', 'name0', 'surname0', 'email0@hotmail.com', '0000000000');